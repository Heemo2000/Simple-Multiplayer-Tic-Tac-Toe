using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class AdsHandler : MonoBehaviour
    {
        [SerializeField]private AdmobManager adManager;
        
        private Coroutine loadAdCoroutine;
        private void LoadRewaredAd()
        {
            if(loadAdCoroutine == null)
            {
                loadAdCoroutine = StartCoroutine(LoadRewardedAdAsync());
            }
        }
        private IEnumerator LoadRewardedAdAsync()
        {
            yield return new WaitUntil(()=> adManager.IsFullyInitialized);
            adManager.LoadRewaredAd();
            loadAdCoroutine = null;
        }
        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnGameStarted += LoadRewaredAd;
            adManager.OnAdFullScreenContentClosed += GameManager.Instance.RematchServerRpc;
        }

        private void OnDestroy() 
        {
            GameManager.Instance.OnGameStarted -= adManager.LoadRewaredAd;
            adManager.OnAdFullScreenContentClosed -= GameManager.Instance.RematchServerRpc;
        }
    }
}
