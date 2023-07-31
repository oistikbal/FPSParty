using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public class JoinUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lobbyContent;

        private LobbyJoin[] _lobbies;

        private void Awake()
        {
            _lobbies = _lobbyContent.GetComponentsInChildren<LobbyJoin>(true);

            SignalsService.GetStream("MainMenuUI", "RefreshLobby").OnSignal += OnRefresh;

            if (!SteamClient.IsValid) 
            {
            }
        }

        private void OnRefresh(Signal signal)
        {
            if (SteamClient.IsValid)
            {
                foreach(var lobby in _lobbies)
                {
                    lobby.gameObject.SetActive(false);
                }

                var query = SteamMatchmaking.LobbyList;
                query.WithMaxResults(50).FilterDistanceWorldwide().WithSlotsAvailable(1);

                query.RequestAsync().ContinueWith(OnContinue, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void OnContinue(Task<Lobby[]> taskLobby)
        {
            var lobbies = taskLobby.Result.OrderBy(lobby => lobby.MemberCount).Reverse().ToArray();

            for (int i = 0; i < lobbies.Length; i++)
            {
                _lobbies[i].SetLobby(lobbies[i]);
            }
            
        }
    }
}
