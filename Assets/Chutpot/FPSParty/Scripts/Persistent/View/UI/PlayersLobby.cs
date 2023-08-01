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
using Utils;

namespace Chutpot.FPSParty.Persistent
{
    public class PlayersLobby : MonoBehaviour
    {
        private PlayerCard[] _playerCards;
        private Texture _defaultImage;

        //clientId, Index pair
        private readonly Dictionary<ulong, int> _seatedPlayers = new Dictionary<ulong, int>(8);
        private readonly PriorityQueue<int, int> _freeSeats = new PriorityQueue<int, int>(8);

        private void Start()
        {
            _playerCards = GetComponentsInChildren<PlayerCard>(true);
            _defaultImage = _playerCards[0].Image.texture;
            RestartLobby();
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdatePlayers").OnSignal += OnUpdateLobby;
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
            //When a client disconnects
            else if(signal.TryGetValue<ulong>(out ulong id))
            {
                _freeSeats.Enqueue(_seatedPlayers[id], _seatedPlayers[id]);
                _playerCards[_seatedPlayers[id]].gameObject.SetActive(false);
                _seatedPlayers.Remove(id);
            }
        }

        private void UpdateClient(FPSClient fpsClient)
        {
            switch (fpsClient.Status)
            {
                //Update client when firstly logged on or when doesnt have and name for speacially when host create server
                case FPSClientStatus.Unready:
                    if (SteamClient.IsValid && !_seatedPlayers.ContainsKey(fpsClient.Id))
                    {
                        _seatedPlayers[fpsClient.Id] = _freeSeats.Dequeue();
                        var steamId = new SteamId();
                        steamId.Value = fpsClient.SteamId;
                        var steamClient = new Friend(steamId);
                        _playerCards[_seatedPlayers[fpsClient.Id]].PlayerName.text = steamClient.Name;
                        _playerCards[_seatedPlayers[fpsClient.Id]].gameObject.SetActive(true);

                        _playerCards[_seatedPlayers[fpsClient.Id]].Image.texture = steamClient.GetMediumAvatarAsync().Result.Value.Covert();
                    }
                    else if(!_seatedPlayers.ContainsKey(fpsClient.Id))
                    {
                        _seatedPlayers[fpsClient.Id] = _freeSeats.Dequeue();
                        _playerCards[_seatedPlayers[fpsClient.Id]].gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RestartLobby()
        {
            Debug.Log("RestartLobby");
            _freeSeats?.Clear();
            _seatedPlayers?.Clear();

            for (int i = 0; i < LobbyNetworkHandler.MaxPlayer; i++)
                _freeSeats.Enqueue(i,i);

            for(int i = 0; i < LobbyNetworkHandler.MaxPlayer; i++)
                _playerCards[i].gameObject.SetActive(false);

        }

        /*
        private void GetImage(Task<Steamworks.Data.Image?> image)
        {
            _image.texture = image.Result.Value.Covert();
        }
        */
    }
}
