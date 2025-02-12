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
        [SerializeField]private Page loadingPage;
        [SerializeField]private TMP_InputField playerNameInputField;
        [SerializeField]private Button authenticateButton;

        
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
        // Start is called before the first frame update
        void Start()
        {
            uiManager.PushPage(authenticatePage);
            playerNameInputField.onValueChanged.AddListener(CheckName);
            authenticateButton.onClick.AddListener(()=> LobbyManager.Instance.Authenticate(playerNameInputField.text));
            authenticateButton.onClick.AddListener(()=> uiManager.PushPage(loadingPage));
        }
    }
}
