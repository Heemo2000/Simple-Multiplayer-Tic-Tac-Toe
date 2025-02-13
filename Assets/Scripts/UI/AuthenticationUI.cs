using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.LobbyManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class AuthenticationUI : MonoBehaviour
    {
        [SerializeField]private UIManager uiManager;
        [SerializeField]private Page authenticatePage;
        [SerializeField]private ProcessingPage processingPage;
        [SerializeField]private Page lobbyOptionsPage;
        [SerializeField]private TMP_InputField playerNameInputField;
        [SerializeField]private Button authenticateButton;

        
        private void ShowLoadingScreen()
        {
            uiManager.PushPage(processingPage);
            processingPage.SetProcessingText("AUTHENTICATING");
        }

        private void DoAuthentication()
        {
            LobbyManager.Instance.Authenticate(playerNameInputField.text);
        }

        private void CloseProcessingPage()
        {
            if(uiManager.IsPageOnTopOfStack(processingPage))
            {
                uiManager.PopPage();
            }
        }
        private void CheckName(string name)
        {
            if(CommonUtility.IsStringValid(name, Constants.ACCEPTED_CHARACTERS, Constants.MAX_NAME_LENGTH))
            {
                authenticateButton.interactable = true;
            }
            else
            {
                authenticateButton.interactable = false;
            }
        }

        private void OpenLobbyOptionsUI()
        {
            if(uiManager.IsPageOnTopOfStack(processingPage))
            {
                uiManager.PopPage();
            }
            uiManager.PushPage(lobbyOptionsPage);
        }
        // Start is called before the first frame update
        void Start()
        {
            uiManager.PushPage(authenticatePage);
            authenticateButton.interactable = false;
            playerNameInputField.onValueChanged.AddListener(CheckName);
            authenticateButton.onClick.AddListener(ShowLoadingScreen);
            authenticateButton.onClick.AddListener(DoAuthentication);
            LobbyManager.Instance.OnSignInSuccess += CloseProcessingPage;
            LobbyManager.Instance.OnSignInSuccess += OpenLobbyOptionsUI;
        }

        private void OnDestroy() 
        {
            playerNameInputField.onValueChanged.RemoveListener(CheckName);
            authenticateButton.onClick.RemoveListener(ShowLoadingScreen);
            authenticateButton.onClick.RemoveListener(DoAuthentication);
            LobbyManager.Instance.OnSignInSuccess -= CloseProcessingPage;
            LobbyManager.Instance.OnSignInSuccess -= OpenLobbyOptionsUI;   
        }
    }
}
