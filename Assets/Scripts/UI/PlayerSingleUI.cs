using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.LobbyManagement;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

namespace Game.UI
{
    public class PlayerSingleUI : MonoBehaviour
    {
        [SerializeField]private TMP_Text playerNameText;
        [SerializeField]private Button kickPlayerButton;
        private Player player;
        private ObjectPool<PlayerSingleUI> singleUIPool;

        public ObjectPool<PlayerSingleUI> SingleUIPool { get => singleUIPool; set => singleUIPool = value; }

        public void Destroy()
        {
            if(singleUIPool != null)
            {
                singleUIPool.Release(this);
            }
        }

        public void UpdatePlayer(Player player)
        {
            this.player = player;
            if(!player.Data.ContainsKey(Constants.KEY_PLAYER_NAME))
            {
                Debug.LogError("Player doesn't contain KEY_PLAYER_NAME");
                return;
            }
            playerNameText.text = player.Data[Constants.KEY_PLAYER_NAME].Value;
        }

        public void SetKickPlayerButtonVisible(bool visible) 
        {
            kickPlayerButton.gameObject.SetActive(visible);
        }

        private void KickPlayer()
        {
            if(player != null)
            {
                LobbyManager.Instance.KickPlayer(player.Id);
            }
        }
        private void Awake() 
        {
            kickPlayerButton.onClick.AddListener(KickPlayer);    
        }
    }
}
