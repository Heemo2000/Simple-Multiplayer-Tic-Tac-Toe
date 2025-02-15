using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace Game.Core
{
    public class NetworkSceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            var progress = NetworkManager.Singleton.SceneManager.LoadScene(Constants.GAME_SCENE_NAME, LoadSceneMode.Single);
        }
    }
}
