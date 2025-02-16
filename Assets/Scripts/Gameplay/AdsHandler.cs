using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class AdsHandler : MonoBehaviour
    {
        
        
        // Start is called before the first frame update
        void Start()
        {
            AdmobManager.Instance.OnAdFullScreenContentClosed += GameManager.Instance.RematchServerRpc;
        }

        private void OnDestroy() 
        {
            AdmobManager.Instance.OnAdFullScreenContentClosed -= GameManager.Instance.RematchServerRpc;
        }
    }
}
