using System.Collections;
using System.Collections.Generic;
using Game.LobbyManagement;
using Game.UI;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.UI
{
    public class LobbyListUI : MonoBehaviour
    {
        [SerializeField]private LobbyListSingleUI lobbyListSingleUIPrefab;
        [SerializeField]private RectTransform content;
        [Min(5)]
        [SerializeField]private int poolSize = 10;

        private ObjectPool<LobbyListSingleUI> lobbyUIPool = null;
        private List<LobbyListSingleUI> activeSingleUIs;


        private void DisplayLists(List<Lobby> lobbies)
        {
            Reset();

            float positionY = 0.0f;
            foreach(Lobby lobby in lobbies)
            {
                var singleUI = lobbyUIPool.Get();
                singleUI.LobbyUIPool = lobbyUIPool;
                RectTransform rectTransform = singleUI.transform.GetComponent<RectTransform>();
                rectTransform.pivot = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.localPosition = new Vector3(0.0f, positionY, 0.0f);
                positionY -= rectTransform.rect.size.y;
                singleUI.UpdateLobby(lobby);
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
        private LobbyListSingleUI CreateSingleUI()
        {
            var singleUI = Instantiate(lobbyListSingleUIPrefab, content.transform);
            singleUI.transform.SetParent(content.transform, false);
            return singleUI;
        }

        private void OnSingleUIGet(LobbyListSingleUI singleUI)
        {
            if(activeSingleUIs.IndexOf(singleUI) == -1)
            {
                activeSingleUIs.Add(singleUI);
            }
            singleUI.gameObject.SetActive(true);
        }

        private void OnSingleUIRelease(LobbyListSingleUI singleUI)
        {
            int index = activeSingleUIs.IndexOf(singleUI);
            if(index != -1)
            {
                activeSingleUIs.RemoveAt(index);
            }
            singleUI.gameObject.SetActive(false);
        }

        private void OnSingleUIDestroy(LobbyListSingleUI singleUI)
        {
            Destroy(singleUI.gameObject);
        }

        private void Awake() 
        {
            activeSingleUIs = new List<LobbyListSingleUI>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if(lobbyUIPool == null)
            {
                lobbyUIPool = new ObjectPool<LobbyListSingleUI>(CreateSingleUI, OnSingleUIGet, OnSingleUIRelease, OnSingleUIDestroy, true, poolSize, poolSize);
            }
        }

        private void OnEnable() 
        {
            if(Application.isPlaying)
            {
                Reset();
                LobbyManager.Instance.OnLobbyListChanged += DisplayLists;
            }   
        }

        private void OnDisable() 
        {
            if(Application.isPlaying)
            {
                LobbyManager.Instance.OnLobbyListChanged -= DisplayLists;
            }
        }
    }
}
