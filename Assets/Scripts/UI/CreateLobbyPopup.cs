using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using Game.Core;
using Game.LobbyManagement;

namespace Game.UI
{
    public class CreateLobbyPopup : Popup
    {
        [SerializeField]private UIManager uiManager;
        [SerializeField]private ProcessingPage processingPage;
        [SerializeField]private TMP_InputField lobbyNameInputField;
        [SerializeField]private TMP_Dropdown lobbyTypeDropdown;

        
        private void OpenProcessingPage()
        {
            uiManager.PushPage(processingPage);
            processingPage.SetProcessingText("CREATING LOBBY");
        }

        private void CheckLobbyName(string name)
        {
            if(CommonUtility.IsStringLengthInRange(name, 20))
            {
                base.okButton.interactable = true;
            }
            else
            {
                base.okButton.interactable = false;
            }
        }

        private void Hide()
        {
            if(uiManager.IsPageOnTopOfStack(this))
            {
                uiManager.PopPage();
            }
        }

        private void HandleLobbyCreation()
        {
            string lobbyName = lobbyNameInputField.text;
            bool isPrivate = (lobbyTypeDropdown.options[lobbyTypeDropdown.value].text == "PRIVATE") ? true : false;

            LobbyManager.Instance.CreateLobby(lobbyName, isPrivate);
        }

        void OnEnable()
        {
            lobbyNameInputField.text = "Default Lobby";
            lobbyTypeDropdown.ClearOptions();
            lobbyTypeDropdown.AddOptions(new List<string>{ "PRIVATE", "PUBLIC"});
            lobbyNameInputField.onValueChanged.AddListener(CheckLobbyName);
            base.okButton.onClick.AddListener(HandleLobbyCreation);
            base.okButton.onClick.AddListener(OpenProcessingPage);
            base.okButton.onClick.AddListener(Hide);
            base.cancelButton.onClick.AddListener(Hide);
            //LobbyManager.Instance.OnJoinedLobby += 
        }

        private void OnDisable() 
        {
            lobbyNameInputField.onValueChanged.RemoveListener(CheckLobbyName);
            base.okButton.onClick.RemoveListener(HandleLobbyCreation);
            base.okButton.onClick.RemoveListener(OpenProcessingPage);
            base.okButton.onClick.RemoveListener(Hide);
            base.cancelButton.onClick.RemoveListener(Hide);
        }
    }
}
