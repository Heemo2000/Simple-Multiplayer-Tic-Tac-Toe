using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;

using Game.Gameplay;

namespace Game.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField]private Image statusBackground;
        [SerializeField]private TMP_Text statusText;
        [SerializeField]private Color winColor;
        [SerializeField]private Color loseColor;
        [SerializeField]private Color tiedColor;
        [SerializeField]private Button rematchButton;
        [SerializeField]private AdmobManager adManager;

        private void Hide()
        {
            statusBackground.gameObject.SetActive(false);
            statusText.gameObject.SetActive(false);
            rematchButton.gameObject.SetActive(false);
        }
        private void UnHide()
        {
            statusBackground.gameObject.SetActive(true);
            statusText.gameObject.SetActive(true);
            rematchButton.gameObject.SetActive(true);
        }
        
        private void ShowWinStatus(Line line, MarkType winPlayerType)
        {
            UnHide();

            if(winPlayerType == GameManager.Instance.LocalPlayerType)
            {
                statusText.text = "YOU WIN!!";
                statusText.color = winColor;
            }
            else
            {
                statusText.text = "YOU LOSE!!";
                statusText.color = loseColor;
            }
        }

        private void ShowTiedMessage()
        {
            UnHide();
            statusText.text = "TIED!!";
            statusText.color = tiedColor;
        }

        private void Awake() {
            GameManager.Instance.OnRematch += Hide;
        }

        private void Start() 
        {
            Hide();
            rematchButton.onClick.AddListener(GameManager.Instance.RematchServerRpc);
            //rematchButton.onClick.AddListener(adManager.ShowRewardedAd);
            GameManager.Instance.OnGameWin += ShowWinStatus;
            GameManager.Instance.OnGameTied += ShowTiedMessage;    
        }

        

        private void OnDestroy() 
        {
            GameManager.Instance.OnRematch -= Hide;
            GameManager.Instance.OnGameWin -= ShowWinStatus;
            GameManager.Instance.OnGameTied -= ShowTiedMessage;
            rematchButton.onClick.RemoveListener(GameManager.Instance.RematchServerRpc);
            //rematchButton.onClick.RemoveListener(adManager.ShowRewardedAd);
        }
    }
}
