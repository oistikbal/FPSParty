using Chutpot.FPSParty.Persistent;
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
using static System.Net.Mime.MediaTypeNames;

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
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect").OnSignal += OnLobbyDisconnect;
        }

        private void OnUpdateLobby(Signal signal)
        {    
            if (signal.TryGetValue<NetworkListEvent<FPSClient>>(out NetworkListEvent<FPSClient> fpsClient))
            {
                UpdateClient(fpsClient.Value);
            }
            else if(signal.TryGetValue<IEnumerator<FPSClient>>(out IEnumerator<FPSClient> clients))
            {
                while(clients.MoveNext())
                {
                    UpdateClient(clients.Current);
                }
            }
        }

        private void OnLobbyDisconnect(Signal signal)
        {
            foreach(var player in _playersGO)
            {
                player.SetActive(false);
            }
        }

        private void UpdateClient(FPSClient fpsClient)
        {
            switch (fpsClient.Status)
            {
                //Update client when firstly logged on or when doesnt have and name for speacially when host create server
                case FPSClientStatus.Unready:
                    if ((SteamClient.IsValid && !_playersGO[fpsClient.Id].activeSelf) || string.IsNullOrEmpty(_playerNames[fpsClient.Id].text))
                    {
                        var steamId = new SteamId();
                        steamId.Value = fpsClient.SteamId;
                        var steamClient = new Friend(steamId);
                        _playersGO[fpsClient.Id].SetActive(true);

                        _playerNames[fpsClient.Id].text = steamClient.Name;
                        _playerImages[fpsClient.Id].texture = steamClient.GetMediumAvatarAsync().Result.Value.Covert();
                    }
                    else
                    {
                        _playersGO[fpsClient.Id].SetActive(true);
                    }
                    break;
                case FPSClientStatus.Off:
                    _playersGO[fpsClient.Id].SetActive(false);
                    break;
                default:
                    break;
            }
        }

        /*
        private void GetImage(Task<Steamworks.Data.Image?> image)
        {
            _image.texture = image.Result.Value.Covert();
        }
        */
    }
}
