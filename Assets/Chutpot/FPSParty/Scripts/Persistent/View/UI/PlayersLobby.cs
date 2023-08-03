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
using static System.Net.Mime.MediaTypeNames;

namespace Chutpot.FPSParty.Persistent
{
    public class PlayersLobby : MonoBehaviour
    {
        private PlayerCard[] _playerCards;
        private Texture _defaultImage;

        //clientId, Index pair
        private readonly Dictionary<ulong, int> _seatedPlayers = new Dictionary<ulong, int>(LobbyNetworkHandler.MaxPlayer);
        private readonly PriorityQueue<int, int> _freeSeats = new PriorityQueue<int, int>(LobbyNetworkHandler.MaxPlayer);

        private void Start()
        {
            _playerCards = GetComponentsInChildren<PlayerCard>(true);
            RestartLobby();
            _defaultImage = _playerCards[0].Image.texture;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdatePlayer").OnSignal += OnUpdateLobby;
        }

        private void OnUpdateLobby(Signal signal)
        {    
            if (signal.TryGetValue<NetworkListEvent<FPSClient>>(out NetworkListEvent<FPSClient> fpsClient))
            {
                UpdateClient(fpsClient);
            }
            else if(signal.TryGetValue<NetworkList<FPSClient>>(out NetworkList<FPSClient> fpsClients))
            {
                RestartLobby();
                UpdateClients(fpsClients);
            }
        }

        private void UpdateClients(NetworkList<FPSClient> fpsClients)
        {
            foreach(var fpsClient in fpsClients)
            {
                if (SteamClient.IsValid && !_seatedPlayers.ContainsKey(fpsClient.Id))
                {
                    _seatedPlayers[fpsClient.Id] = _freeSeats.Dequeue();
                    var steamId = new SteamId();
                    steamId.Value = fpsClient.SteamId;
                    var steamClient = new Friend(steamId);
                    _playerCards[_seatedPlayers[fpsClient.Id]].PlayerName.text = steamClient.Name;
                    _playerCards[_seatedPlayers[fpsClient.Id]].gameObject.SetActive(true);
                    if(fpsClient.Status == FPSClientStatus.Unready)
                        _playerCards[_seatedPlayers[fpsClient.Id]].PlayerStatus.isOn = false;
                    else
                        _playerCards[_seatedPlayers[fpsClient.Id]].PlayerStatus.isOn = true;
                    GetImage(steamClient, _playerCards[_seatedPlayers[fpsClient.Id]].Image);
                }
                else if (!_seatedPlayers.ContainsKey(fpsClient.Id))
                {
                    _seatedPlayers[fpsClient.Id] = _freeSeats.Dequeue();
                    _playerCards[_seatedPlayers[fpsClient.Id]].gameObject.SetActive(true);
                    if (fpsClient.Status == FPSClientStatus.Unready)
                        _playerCards[_seatedPlayers[fpsClient.Id]].PlayerStatus.isOn = false;
                    else
                        _playerCards[_seatedPlayers[fpsClient.Id]].PlayerStatus.isOn = true;
                }
            }
        }

        private void UpdateClient(NetworkListEvent<FPSClient> fpsClient)
        {
            switch (fpsClient.Type)
            {
                case NetworkListEvent<FPSClient>.EventType.Add:
                    if (SteamClient.IsValid && !_seatedPlayers.ContainsKey(fpsClient.Value.Id))
                    {
                        _seatedPlayers[fpsClient.Value.Id] = _freeSeats.Dequeue();
                        var steamId = new SteamId();
                        steamId.Value = fpsClient.Value.SteamId;
                        var steamClient = new Friend(steamId);
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].PlayerName.text = steamClient.Name;
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].gameObject.SetActive(true);
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].PlayerStatus.isOn = false;
                        GetImage(steamClient, _playerCards[_seatedPlayers[fpsClient.Value.Id]].Image);
                        break;
                    }
                    else if(!_seatedPlayers.ContainsKey(fpsClient.Value.Id))
                    {
                        _seatedPlayers[fpsClient.Value.Id] = _freeSeats.Dequeue();
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].gameObject.SetActive(true);
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].PlayerStatus.isOn = false;
                        break;
                    }
                    break;
                case NetworkListEvent<FPSClient>.EventType.Remove:
                    _freeSeats.Enqueue(_seatedPlayers[fpsClient.Value.Id], _seatedPlayers[fpsClient.Value.Id]);
                    _playerCards[_seatedPlayers[fpsClient.Value.Id]].gameObject.SetActive(false);
                    _seatedPlayers.Remove(fpsClient.Value.Id);
                    break;
                case NetworkListEvent<FPSClient>.EventType.Value:
                    if (fpsClient.Value.Status == FPSClientStatus.Unready)
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].PlayerStatus.isOn = false;
                    else if(fpsClient.Value.Status == FPSClientStatus.Ready)
                        _playerCards[_seatedPlayers[fpsClient.Value.Id]].PlayerStatus.isOn = true;
                    break;
                default:
                    break;
            }
        }

        private void RestartLobby()
        {
            _freeSeats?.Clear();
            _seatedPlayers?.Clear();

            for (int i = 0; i < LobbyNetworkHandler.MaxPlayer; i++)
                _freeSeats.Enqueue(i,i);

            for(int i = 0; i < LobbyNetworkHandler.MaxPlayer; i++)
            {
                _playerCards[i].gameObject.SetActive(false);
            }


        }

        private async void GetImage(Friend steamClient, RawImage image)
        {
            var tex = await steamClient.GetMediumAvatarAsync();
            image.texture = tex.Value.Covert();
        }

    }
}
