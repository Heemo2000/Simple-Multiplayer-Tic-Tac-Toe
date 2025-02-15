using UnityEngine;
using Game.Core;
using Game.LobbyManagement;
using Unity.Services.Lobbies.Models;
using Game.UI;
namespace Game.Gameplay
{
    public class StartGameManager : MonoBehaviour
    {
        [SerializeField]private NetworkSceneLoader sceneLoader;
        [SerializeField]private UIManager uiManager;
        [SerializeField]private ProcessingPage processingPage;
        private void StartGame(Lobby lobby)
        {
            uiManager.PushPage(processingPage);
            processingPage.SetProcessingText("STARTING GAME");
            sceneLoader.LoadScene(Constants.GAME_SCENE_NAME);
        }

        // Start is called before the first frame update
        void Start()
        {
            LobbyManager.Instance.OnLobbyStartGame += StartGame;
        }

        private void OnDestroy() 
        {
            LobbyManager.Instance.OnLobbyStartGame -= StartGame;    
        }
    }
}
