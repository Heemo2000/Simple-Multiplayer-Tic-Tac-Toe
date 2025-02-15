using Game.LobbyManagement;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace Game.UI
{
    public class LobbyOptionsUI : MonoBehaviour
    {
        [SerializeField]private UIManager uiManager;
        [SerializeField]private Button createLobbyButton;
        [SerializeField]private Button joinLobbyButton;
        [SerializeField]private Button quickJoinLobbyButton;
        [SerializeField]private ProcessingPage processingPage;
        [SerializeField]private CreateLobbyPopup createLobbyPopup;
        [SerializeField]private JoinLobbyByCodePopup joinLobbyPopup;
        [SerializeField]private Page lobbyUIPage;
        [SerializeField]private LobbyUI lobbyUI;
        

        private void OpenCreatePopup()
        {
            uiManager.PushPage(createLobbyPopup);
        }

        private void OpenJoinLobbyPopup()
        {
            uiManager.PushPage(joinLobbyPopup);
        }

        private void OpenProcessingPage()
        {
            uiManager.PushPage(processingPage);
            processingPage.SetProcessingText("FINDING LOBBY");
        }

        private void CloseProcessingPage()
        {
            if(uiManager.IsPageOnTopOfStack(processingPage))
            {
                uiManager.PopPage();
            }
        }

        private void OpenLobbyUIPage(Lobby lobby)
        {
            if(uiManager.IsPageOnTopOfStack(processingPage))
            {
                uiManager.PopPage();
            }

            uiManager.PushPage(lobbyUIPage);
            lobbyUI.SetJoinCodeText(LobbyManager.Instance.LobbyCode);
        }

        private void QuickJoinLobby()
        {
            LobbyManager.Instance.QuickJoinLobby();
        }

        // Start is called before the first frame update
        void Start()
        {
            createLobbyButton.onClick.AddListener(OpenCreatePopup);
            joinLobbyButton.onClick.AddListener(OpenJoinLobbyPopup);
            quickJoinLobbyButton.onClick.AddListener(OpenProcessingPage);
            quickJoinLobbyButton.onClick.AddListener(QuickJoinLobby);
            LobbyManager.Instance.OnJoinedLobby += OpenLobbyUIPage;
            LobbyManager.Instance.OnJoinLobbyFailed += CloseProcessingPage;
        }

        private void OnDestroy() 
        {
            createLobbyButton.onClick.RemoveListener(OpenCreatePopup);
            joinLobbyButton.onClick.RemoveListener(OpenJoinLobbyPopup);
            quickJoinLobbyButton.onClick.RemoveListener(OpenProcessingPage);
            quickJoinLobbyButton.onClick.RemoveListener(QuickJoinLobby);
            LobbyManager.Instance.OnJoinedLobby -= OpenLobbyUIPage;
            LobbyManager.Instance.OnJoinLobbyFailed -= CloseProcessingPage;
        }
    }
}
