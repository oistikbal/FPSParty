using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Steamworks.Data;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Netcode.Transports.Facepunch;
using System.Threading.Tasks;
using System;
using Doozy.Runtime.Signals;

namespace Chutpot.FPSParty.Persistent
{
    public class FPNetworkService : AbstractNetworkService
    {
        private FacepunchTransport _facepunchTransport;
        private HostCreateData _hostCreateData;
        private Lobby _lobby;

        public override void Initialize()
        {
            base.Initialize();

            _facepunchTransport = NetworkManager.Singleton.GetComponent<FacepunchTransport>();

            PlayerModel.Id = SteamClient.SteamId;
            PlayerModel.ProfileImage = GetAvatar(SteamClient.SteamId).Result;
            PlayerModel.Name = SteamClient.Name;

            Steamworks.SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            Steamworks.SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            Steamworks.SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            Steamworks.SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            Steamworks.SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
            Steamworks.SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        }

        protected override void OnClientStopped(bool obj)
        {
            base.OnClientStopped(obj);
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect").SendSignal();
        }

        protected override void OnClientStarted()
        {
            base.OnClientStarted();
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobbySuccesfull").SendSignal();
        }

        protected override async void JoinLobby(Signal signal)
        {
            base.JoinLobby(signal);
            if (NetworkManager.Singleton.IsConnectedClient)
                StopOrLeave();

            signal.TryGetValue<Lobby>(out Lobby lobby);

            UIPopup.ClearQueue();
            var popup = UIPopup.Get("PopupBlock");
            popup.SetTexts("Joining the game.");
            popup.Show();

            RoomEnter joinedLobby = await lobby.Join();
            popup.Hide();

            if (joinedLobby != RoomEnter.Success)
            {
                Debug.Log("Failed to join the lobby.");
                StopOrLeave();
            }
            else
            {
                _lobby = lobby;
                _facepunchTransport.targetSteamId = _lobby.Id;
                StartClient();
            }
        }

        protected override void StopOrLeave()
        {
            base.StopOrLeave();
            if (_lobby.Id != 0)
                _lobby.Leave();

            _lobby = new Lobby();
        }

        protected override async void OnHostCreateSignal(Signal signal)
        {
            base.OnHostCreateSignal(signal);
            signal.TryGetValue<HostCreateData>(out var hostCreate);

            _hostCreateData = hostCreate;

            UIPopup.ClearQueue();
            var popup = UIPopup.Get("PopupBlock");
            popup.SetTexts("Creating lobby...");
            popup.Show();

            _lobby = (Lobby)await SteamMatchmaking.CreateLobbyAsync(LobbyNetworkHandler.MaxPlayer);

            if (hostCreate.name == string.Empty)
                _lobby.SetData("Name", _lobby.Owner.Name + "`s lobby");
            else
                _lobby.SetData("Name", hostCreate.name);
            popup.Hide();
        }

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
            _lobbyHandler.Lobby = lobby;
        }

        private void OnLobbyGameCreated(Lobby lobby, uint arg2, ushort arg3, SteamId arg4)
        {
            //last entry point of lobby creating
            Debug.Log("OnLobbyGameCreated");
            _lobbyHandler.GetComponent<NetworkObject>().Spawn();
        }

        private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            StopOrLeave();

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
        }

        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log("OnLobbyEntered");
            _lobby = lobby;
            if (NetworkManager.Singleton.IsHost)
                return;

            //Start Client for non-host
            UITag.GetFirstTag("MainMenuUI", "LobbyHandler").GetComponent<LobbyNetworkHandler>().Lobby = _lobby;
            StartClient();
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
