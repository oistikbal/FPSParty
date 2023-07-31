using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chutpot.FPSParty
{
    public class LobbyJoin : MonoBehaviour
    {
        [SerializeField]
        private RawImage _image;
        [SerializeField]
        private TextMeshProUGUI _lobbyName;
        [SerializeField]
        private TextMeshProUGUI _mapName;
        [SerializeField]
        private TextMeshProUGUI _playerCount;

        public void SetLobby(Lobby lobby)
        {
            gameObject.SetActive(true);
            _lobbyName.text = lobby.GetData("name");
            _mapName.text = lobby.GetData("map");
            _playerCount.text = lobby.MemberCount.ToString() + "/" + lobby.MaxMembers;
            SteamFriends.GetLargeAvatarAsync(lobby.Owner.Id).ContinueWith(GetImage, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void GetImage(Task<Steamworks.Data.Image?> image)
        {
            _image.texture = image.Result.Value.Covert();
        }
    }
}
