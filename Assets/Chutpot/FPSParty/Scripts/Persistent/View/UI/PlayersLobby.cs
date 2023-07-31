using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Chutpot.FPSParty.Persistent
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
        private readonly Dictionary<ulong, GameObject> _activePlayersGO = new Dictionary<ulong, GameObject>(8);
        private readonly Queue<GameObject> _freeGO = new Queue<GameObject>();

        private void Start()
        {
            RestartLobby();
            _defaultImage = _playerImages[0].texture;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdateLobby").OnSignal += OnUpdateLobby;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect").OnSignal += signal => RestartLobby();
        }

        private void OnUpdateLobby(Signal signal)
        {    
            //When Players Gets Updated
            if(signal.TryGetValue<IEnumerator<FPSClient>>(out IEnumerator<FPSClient> clients))
            {
                while(clients.MoveNext())
                {
                    UpdateClient(clients.Current);
                }
            }
            //When a clien disconnects
            else if(signal.TryGetValue<ulong>(out ulong id))
            {
                _freeGO.Enqueue(_activePlayersGO[id]);
                _activePlayersGO[id].SetActive(false);
                _activePlayersGO.Remove(id);
            }
        }

        private void UpdateClient(FPSClient fpsClient)
        {
            switch (fpsClient.Status)
            {
                //Update client when firstly logged on or when doesnt have and name for speacially when host create server
                case FPSClientStatus.Unready:
                    if (SteamClient.IsValid && !_activePlayersGO.ContainsKey(fpsClient.Id))
                    {
                        _activePlayersGO[fpsClient.Id] = _freeGO.Dequeue();
                        var steamId = new SteamId();
                        steamId.Value = fpsClient.SteamId;
                        var steamClient = new Friend(steamId);
                        _activePlayersGO[fpsClient.Id].SetActive(true);

                        _playerNames[fpsClient.Id].text = steamClient.Name;
                        _playerImages[fpsClient.Id].texture = steamClient.GetMediumAvatarAsync().Result.Value.Covert();
                    }
                    else if(!_activePlayersGO.ContainsKey(fpsClient.Id))
                    {
                        _activePlayersGO[fpsClient.Id] = _freeGO.Dequeue();
                        _activePlayersGO[fpsClient.Id].SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RestartLobby()
        {
            _freeGO.Clear();
            _activePlayersGO.Clear();

            foreach (var playerGo in _playersGO)
                _freeGO.Enqueue(playerGo);

            foreach (var player in _playersGO)
                player.SetActive(false);

        }

        /*
        private void GetImage(Task<Steamworks.Data.Image?> image)
        {
            _image.texture = image.Result.Value.Covert();
        }
        */
    }
}
