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
                switch (fpsClient.Value.Status)
                {
                    //Update client when firstly logged on or when doesnt have and name for speacially when host create server
                    case FPSClientStatus.Unready:
                        if ((SteamClient.IsValid && !_playersGO[fpsClient.Index].activeSelf) || string.IsNullOrEmpty(_playerNames[fpsClient.Index].text))
                        {
                            var steamId = new SteamId();
                            steamId.Value = fpsClient.Value.SteamId;
                            var steamClient = new Friend(steamId);
                            _playersGO[fpsClient.Index].SetActive(true);

                            _playerNames[fpsClient.Index].text = steamClient.Name;
                            _playerImages[fpsClient.Index].texture = steamClient.GetMediumAvatarAsync().Result.Value.Covert();
                        }
                        else
                        {
                            _playersGO[fpsClient.Index].SetActive(true);
                        }
                        break;
                    case FPSClientStatus.Off:
                        _playersGO[fpsClient.Index].SetActive(false);
                        break;
                    default:
                        break;
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

        /*
        private void GetImage(Task<Steamworks.Data.Image?> image)
        {
            _image.texture = image.Result.Value.Covert();
        }
        */
    }
}
