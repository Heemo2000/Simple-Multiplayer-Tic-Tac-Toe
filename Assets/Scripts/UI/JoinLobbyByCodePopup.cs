using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Game.Core;
using Game.LobbyManagement;

namespace Game.UI
{
    public class JoinLobbyByCodePopup : Popup
    {
        [SerializeField]private UIManager uiManager;
        [SerializeField]private TMP_InputField joinCodeInputField;


        private void CheckJoinCode(string joinCode)
        {
            if(joinCode.Length >= 3 && CommonUtility.IsStringLengthInRange(joinCode, 8))
            {
                base.okButton.interactable = true;
            }
            else
            {
                base.okButton.interactable = false;
            }
        }
        private void HandleJoiningLobbyByCode()
        {
            LobbyManager.Instance.JoinLobbyByCode(joinCodeInputField.text);
        }


        private void Hide()
        {
            if(uiManager.IsPageOnTopOfStack(this))
            {
                uiManager.PopPage();
            }
        }

        void OnEnable()
        {
            joinCodeInputField.text = "Test";
            joinCodeInputField.onValueChanged.AddListener(CheckJoinCode);
            base.okButton.onClick.AddListener(HandleJoiningLobbyByCode);
            base.cancelButton.onClick.AddListener(Hide);
            LobbyManager.Instance.OnJoinLobbyFailed += Hide;
        }

        private void OnDisable() 
        {
            joinCodeInputField.onValueChanged.RemoveListener(CheckJoinCode);
            base.okButton.onClick.RemoveListener(HandleJoiningLobbyByCode);
            base.cancelButton.onClick.RemoveListener(Hide);
            LobbyManager.Instance.OnJoinLobbyFailed -= Hide;
        }
    }
}
