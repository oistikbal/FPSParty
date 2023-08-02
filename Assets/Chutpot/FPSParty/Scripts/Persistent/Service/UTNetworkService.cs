using Chutpot.FPSParty.Persistent;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Chutpot.FPSParty
{
    public class UTNetworkService : AbstractNetworkService
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnHostCreateSignal(Signal signal)
        {
            base.OnHostCreateSignal(signal);
            _lobbyHandler.GetComponent<NetworkObject>().Spawn();
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobbySuccesfull").SendSignal();
        }

        protected override void JoinLobby(Signal signal)
        {
            base.JoinLobby(signal);
            StartClient();
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

    }
}
