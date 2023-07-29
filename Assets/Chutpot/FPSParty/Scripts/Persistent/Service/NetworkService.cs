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
using System.Linq.Expressions;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using DG.Tweening.Core.Easing;

namespace Chutpot.FPSParty.Persistent
{
    [Serializable]
    public struct HostCreate
    {
        public string name;
        public bool isInvitationOnly;

        public HostCreate(string name, bool isInvitationOnly)
        {
            this.name = name;
            this.isInvitationOnly = isInvitationOnly;
        }

        public override string ToString()
        {
            return "HostName: " + name + " invitation: " + isInvitationOnly.ToString();
        }
    }

    public enum FPSClientStatus
    {
        Off,
        Unready,
        Ready,
        Loading,
        InGame
    }

    public class FPSClient 
    {
        public ulong clientId;
        public string name;
        public FPSClientStatus status;
    }

    public class FPSLobby
    {
        public Lobby Lobby;
        public FPSClient[] Clients;
        public bool IsInvitationOnly;
        public bool IsHost;

        public FPSLobby(string serverName, bool isInvitationOnly, bool isHost)
        {
            IsInvitationOnly = isInvitationOnly;
            IsHost = isHost;
            Clients = new FPSClient[8];
        }
    }

    public class NetworkService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private FPSLobby _fpsLobby;

        private NetworkServiceView _networkServiceView;
        private FacepunchTransport _facepunchTransport;

        private const string _networkServiceAddress = "NetworkService";

        [PostConstruct]
        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_networkServiceAddress);
            var op = handle.WaitForCompletion();

            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            _networkServiceView = go.GetComponent<NetworkServiceView>();

            if (_networkServiceView.IsSteamInitialized)
            {
                var playerTask = GetAvatar(SteamClient.SteamId);
                playerTask.Wait();

                PlayerModel.Id = SteamClient.SteamId;
                PlayerModel.ProfileImage = playerTask.Result;
                PlayerModel.Name = SteamClient.Name;
                _facepunchTransport = _networkServiceView.GetComponentInChildren<FacepunchTransport>();
            }

            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "HostCreate").OnSignal += OnHostCreateSignal;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ExitLobby").OnSignal += signal => StopOrLeave();

            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

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
            signal.TryGetValue<HostCreate>(out var hostCreate);

            _fpsLobby = new FPSLobby(hostCreate.name, hostCreate.isInvitationOnly, true);

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
                popup.SetTexts("Loading.");
                popup.Show();
                _fpsLobby.Lobby = (Lobby)await SteamMatchmaking.CreateLobbyAsync(8);
                popup.Hide();
            }

        }

        // ------------ Custom Actions


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
                _fpsLobby.Lobby = lobby;
            }
        }



        private void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log($"Invite From {friend.Name}");
        }

        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
        }

        private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {

        }

        private void OnLobbyEntered(Lobby lobby)
        {
            if (NetworkManager.Singleton.IsHost)
                return;

            StartClient(_fpsLobby.Lobby.Owner.Id);
        }


        // Host

        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            if(result != Result.OK)
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts($"Failed create the server.\n Status: {result}");
                popup.ShowFromQueue();
            }

            if (!_fpsLobby.IsInvitationOnly)
                lobby.SetPublic();

            lobby.SetJoinable(true);
            lobby.SetGameServer(lobby.Owner.Id);
            Debug.Log(lobby.Owner.Name);
        }

        private void OnLobbyGameCreated(Lobby lobby, uint arg2, ushort arg3, SteamId arg4)
        {
        }

        //-------------- Unity Messages
        private void OnClientDisconnected(ulong clientId)
        {
        }

        private void OnClientConnected(ulong clientId)
        {
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobby").SendSignal();
        }

        private void OnServerStarted()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                OnClientConnected(NetworkManager.Singleton.LocalClientId);
            }
        }

        public void StartClient(ulong targetID)
        {
            _facepunchTransport.targetSteamId = targetID;
            if (!NetworkManager.Singleton.StartClient())
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts("Failed join the game");
                popup.Show();
            }
            _fpsLobby = new FPSLobby("asd", false, false);
        }

        //--------------- Unity Messages

        public void StopOrLeave()
        {
            if(_fpsLobby.Lobby.Id != 0)
                _fpsLobby.Lobby.Leave();

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