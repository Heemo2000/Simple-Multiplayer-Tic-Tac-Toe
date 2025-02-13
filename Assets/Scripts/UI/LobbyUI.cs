using System.Collections;
using System.Collections.Generic;
using Game.LobbyManagement;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField]private PlayerSingleUI playerSingleUIPrefab;
        [SerializeField]private RectTransform content;
        [SerializeField]private TMP_Text joinCodeText;

        [Min(2)]
        [SerializeField]private int poolSize = 2;

        private ObjectPool<PlayerSingleUI> singleUIPool;
        private List<PlayerSingleUI> activeSingleUIs;
        private Lobby lobby;

        public void SetJoinCodeText(string joinCode)
        {
            joinCodeText.text = joinCode;
        }
        private void DisplayPlayerLists()
        {
            DisplayPlayerLists(LobbyManager.Instance.JoinedLobby);
        }

        private void DisplayPlayerLists(Lobby lobby)
        {
            this.lobby = lobby;
            Reset();
            float positionY = 0.0f;

            foreach(Player player in lobby.Players)
            {
                var singleUI = singleUIPool.Get();
                singleUI.SingleUIPool = singleUIPool;
                RectTransform rectTransform = singleUI.transform.GetComponent<RectTransform>();
                rectTransform.pivot = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.localPosition = new Vector3(0.0f, positionY, 0.0f);
                singleUI.SetKickPlayerButtonVisible(LobbyManager.Instance.IsLobbyHost() && player.Id != AuthenticationService.Instance.PlayerId);
                singleUI.UpdatePlayer(player);
            }
        }

        private void Reset()
        {
            while(activeSingleUIs.Count > 0)
            {
                var singleUI = activeSingleUIs[activeSingleUIs.Count - 1];
                singleUI.Destroy();
            }
        }
        private PlayerSingleUI CreateSingleUI()
        {
            var singleUI = Instantiate(playerSingleUIPrefab, content.transform);
            singleUI.transform.SetParent(content.transform, false);
            return singleUI;
        }

        private void OnSingleUIGet(PlayerSingleUI singleUI)
        {
            int index = activeSingleUIs.IndexOf(singleUI);
            if(index == -1)
            {
                activeSingleUIs.Add(singleUI);
            }
            singleUI.gameObject.SetActive(true);
        }

        private void OnSingleUIRelease(PlayerSingleUI singleUI)
        {
            int index = activeSingleUIs.IndexOf(singleUI);
            if(index != -1)
            {
                activeSingleUIs.RemoveAt(index);
            }
            singleUI.gameObject.SetActive(false);
        }

        private void OnSingleUIDestroy(PlayerSingleUI singleUI)
        {
            Destroy(singleUI.gameObject);
        }

        private void Awake() 
        {
            activeSingleUIs = new List<PlayerSingleUI>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if(singleUIPool == null)
            {
                singleUIPool = new ObjectPool<PlayerSingleUI>(CreateSingleUI, 
                                                              OnSingleUIGet, 
                                                              OnSingleUIRelease, 
                                                              OnSingleUIDestroy, 
                                                              true, 
                                                              poolSize, 
                                                              poolSize);

                LobbyManager.Instance.OnJoinedLobby += DisplayPlayerLists;
                LobbyManager.Instance.OnJoinedLobbyUpdate += DisplayPlayerLists;
                LobbyManager.Instance.OnLeftLobby += DisplayPlayerLists;
                LobbyManager.Instance.OnKickedFromLobby += DisplayPlayerLists;
            }
        }


        private void OnDestroy() 
        {
            LobbyManager.Instance.OnJoinedLobby -= DisplayPlayerLists;
            LobbyManager.Instance.OnJoinedLobbyUpdate -= DisplayPlayerLists;
            LobbyManager.Instance.OnLeftLobby -= DisplayPlayerLists;
            LobbyManager.Instance.OnKickedFromLobby -= DisplayPlayerLists;
        }
    }
}
