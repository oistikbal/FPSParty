using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using Steamworks.Data;
using Steamworks;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Steamworks.ServerList;
using Doozy.Runtime.UIManager.Components;
using Unity.Netcode.Transports.UTP;

namespace Chutpot.FPSParty.Persistent
{
    [Serializable]
    public struct HostCreateData
    {
        public string name;
        public bool isInvitationOnly;

        public HostCreateData(string name, bool isInvitationOnly)
        {
            this.name = name;
            this.isInvitationOnly = isInvitationOnly;
        }

        public override string ToString()
        {
            return "HostName: " + name + " invitation: " + isInvitationOnly.ToString();
        }
    }

    public class NetworkService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private NetworkServiceView _networkServiceView;
        private FacepunchTransport _facepunchTransport;
        private UnityTransport _unityTransport;

        private const string _networkServiceAddress = "NetworkService";
        private const string _lobbyNetworkHandlerAddress = "LobbyNetworkHandler";

        private Lobby _lobby;
        private HostCreateData _hostCreateData;

        private GameObject _lobbyHandlerPrefab;
        private LobbyNetworkHandler _lobbyHandler;

        [PostConstruct]
        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_networkServiceAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            _networkServiceView = go.GetComponent<NetworkServiceView>();

            handle = Addressables.LoadAssetAsync<GameObject>(_lobbyNetworkHandlerAddress);
            op = handle.WaitForCompletion();
            _lobbyHandlerPrefab = op;

            if (_networkServiceView.IsSteamInitialized)
            {
                PlayerModel.Id = SteamClient.SteamId;
                PlayerModel.ProfileImage = GetAvatar(SteamClient.SteamId).Result;
                PlayerModel.Name = SteamClient.Name;
                _facepunchTransport = _networkServiceView.GetComponentInChildren<FacepunchTransport>();
            }
            else
            {
                _unityTransport = _networkServiceView.GetComponentInChildren<UnityTransport>();
            }


            NetworkManager.Singleton.OnClientStopped += x => Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect").SendSignal();
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "HostCreate").OnSignal += OnHostCreateSignal;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ExitLobby").OnSignal += signal => StopOrLeave();
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobby").OnSignal += signal => 
            {
                signal.TryGetValue<Lobby>(out Lobby lobby);
                {

                }
                if (_facepunchTransport)
                {
                    lobby.Join();
                }
                else
                {
                    StartClient(0);
                }

            };

            if (_facepunchTransport)
            {
                Steamworks.SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
                Steamworks.SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
                Steamworks.SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
                Steamworks.SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
                Steamworks.SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
                Steamworks.SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
                SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;  
            }
        }


        ~NetworkService()
        {
        }


        // ----------- Custom Actions


        // Entry point of creating host
        private async void OnHostCreateSignal(Signal signal)
        {
            Debug.Log("OnHostCreateSignal");

            signal.TryGetValue<HostCreateData>(out var hostCreate);

            _hostCreateData = hostCreate;

            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (!NetworkManager.Singleton.StartHost())
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts("Failed create the server.");
                popup.Show();
                return;
            }

            if (_facepunchTransport)
            {
                UIPopup.ClearQueue();
                var popup = UIPopup.Get("PopupBlock");
                popup.SetTexts("Creating lobby...");
                popup.Show();

                _lobbyHandler = MonoBehaviour.Instantiate(_lobbyHandlerPrefab).GetComponent<LobbyNetworkHandler>();

                _lobby = (Lobby)await SteamMatchmaking.CreateLobbyAsync(8);

                if (hostCreate.name == string.Empty)
                    _lobby.SetData("Name", _lobby.Owner.Name + "`s lobby");
                else
                     _lobby.SetData("Name", hostCreate.name);
                _lobbyHandler.Lobby = _lobby;
                popup.Hide();
            }
            else
            {
                _lobbyHandler = MonoBehaviour.Instantiate(_lobbyHandlerPrefab).GetComponent<LobbyNetworkHandler>();
                _lobbyHandler.GetComponent<NetworkObject>().Spawn();
            }
        }

        // ------------ Custom Actions

        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            Debug.Log("OnLobbyCreated");
            if (result != Result.OK)
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts($"Failed create the server.\n Status: {result}");
                popup.ShowFromQueue();
            }

            if (!_hostCreateData.isInvitationOnly)
                lobby.SetPublic();
            
            lobby.SetJoinable(true);
            lobby.SetGameServer(lobby.Owner.Id);
            _lobbyHandler.GetComponent<NetworkObject>().Spawn();
        }

        private void OnLobbyGameCreated(Lobby lobby, uint arg2, ushort arg3, SteamId arg4)
        {
            //last entry point of lobby creating
            Debug.Log("OnLobbyGameCreated");
        }

        private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            UIPopup.ClearQueue();
            var popup = UIPopup.Get("PopupBlock");
            popup.SetTexts("Joining Friend's game.");
            popup.Show();

            RoomEnter joinedLobby = await lobby.Join();
            popup.Hide();

            if (joinedLobby != RoomEnter.Success)
            {
                Debug.Log("Failed to join lobby.");
            }
            else
            {
                _lobby = lobby;
            }
        }



        private void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log($"Invite From {friend.Name}");
        }

        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            _lobby = lobby;
        }

        private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            Debug.Log("OnLobbyMemberJoined");
            _lobby = lobby;
        }

        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log("OnLobbyEntered");
            _lobby = lobby;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobbySuccesfull").SendSignal();
            if (NetworkManager.Singleton.IsHost)
                return;

            //Start Client for non-host
            StartClient(lobby.Owner.Id.Value);
        }


        //-------------- NetworkManager Messages
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log("OnClientDisconnected");
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log("OnClientConnected");
            //Send host UI to lobby;
            if (clientId == 0)
            {
                Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobbySuccesfull").SendSignal();
            }
        }

        private void OnServerStarted()
        {
            Debug.Log("OnServerStarted");
        }

        //--------------- NetworkManager Messages

        public void StartClient(ulong targetID)
        {
            Debug.Log("StartClient");
            if (_facepunchTransport)
            {
                _facepunchTransport.targetSteamId = targetID;
            }
            else
            {
                Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobbySuccesfull").SendSignal();
            }


            try
            {
                if (!NetworkManager.Singleton.StartClient())
                {
                    var popup = UIPopup.Get("Popup");
                    popup.SetTexts("Failed to join the game");
                    popup.Show();
                }
            }
            catch(Exception e)
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts("Failed to join the game");
                popup.Show();
                Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ExitLobby").SendSignal();
            }
        }



        public void StopOrLeave()
        {
            
            if(_lobby.Id != 0)
                _lobby.Leave();
            

            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }

            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect").SendSignal();
            NetworkManager.Singleton.Shutdown();
        }


        public async Task<Steamworks.Data.Image?> GetAvatar(SteamId id)
        {
            try
            {
                // Get Avatar using await
                return await SteamFriends.GetLargeAvatarAsync(id);
            }
            catch (Exception e)
            {
                // If something goes wrong, log it
                Debug.Log(e);
                return null;
            }
        }
    }
}