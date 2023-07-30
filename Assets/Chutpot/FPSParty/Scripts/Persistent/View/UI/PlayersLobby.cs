using Chutpot.FPSParty.Persistent;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Steamworks.Data;
using Steamworks.ServerList;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Chutpot.FPSParty
{

    public class PlayersLobby : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] _playerNames;
        [SerializeField]
        private RawImage[] _playerImages;
        [SerializeField]
        private GameObject[] _playersGO;

        private Texture _defaultImage;

        private void Start()
        {
            _defaultImage = _playerImages[0].texture;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdateLobby").OnSignal += OnUpdateLobby;
        }

        private void OnUpdateLobby(Signal signal)
        {    
            if (signal.TryGetValue<NetworkList<FPSClient>>(out NetworkList<FPSClient> clients))
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    _playersGO[clients[i].Id].SetActive(true);

                    /*
                    _playersGO[client.clientId].SetActive(true);
                    _playerNames[client.clientId].text = _fpsLobby.Clients[client.clientId].friend.Name;
                    _playerImages[client.clientId].texture = _defaultImage;
                    _playerImages[client.clientId].texture = _fpsLobby.Clients[client.clientId].friend.GetMediumAvatarAsync().Result.Value.Covert();
                    */
                }
                
                /*
                foreach (var client in _fpsLobby.Clients.Where(x => x.status == FPSClientStatus.Off))
                {

                    _playersGO[client.clientId].SetActive(false);
                }
                */
            }
            
        }
    }
}
