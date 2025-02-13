using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using Game.LobbyManagement;
using UnityEngine.Pool;
namespace Game.UI
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        [SerializeField]private TMP_Text lobbyNameText;
        [SerializeField]private TMP_Text membersText;
        private Lobby lobby;
        private Button button;
        private ObjectPool<LobbyListSingleUI> lobbyUIPool;

        public ObjectPool<LobbyListSingleUI> LobbyUIPool { get => lobbyUIPool; set => lobbyUIPool = value; }


        public void Destroy()
        {
            if(lobbyUIPool != null)
            {
                lobbyUIPool.Release(this);
            }
        }

        public void UpdateLobby(Lobby lobby)
        {
            this.lobby = lobby;
            lobbyNameText.text = lobby.Name;
            membersText.text = lobby.Players.Count.ToString() + "/" +  lobby.MaxPlayers.ToString();
        }

        private void Awake() 
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(()=> LobbyManager.Instance.JoinLobby(lobby));
        }
    }
}
