using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay
{
    public class ParticleSystemController : MonoBehaviour
    {
        [SerializeField]private ParticleSystem[] particleSystems;
        private Coroutine playCoroutine;
        private NetworkObject networkObject;
        private NetworkObjectPool networkObjectPool;
        private GameObject prefab = null;
        public NetworkObjectPool NetworkObjectPool { get => networkObjectPool; set => networkObjectPool = value; }
        public GameObject Prefab { get => prefab; set => prefab = value; }

        public void Play()
        {
            if(playCoroutine == null)
            {
                playCoroutine = StartCoroutine(PlayParticle());
            }
        }

        private IEnumerator PlayParticle()
        {
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            }

            yield return new WaitUntil(()=> IsAllStopped());

            playCoroutine = null;
            if(networkObjectPool != null)
            {
                networkObjectPool.ReturnNetworkObject(networkObject, prefab);
            }
        }

        private bool IsAllStopped()
        {
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                if(particleSystem.IsAlive())
                {
                    return false;
                }
            }

            return true;
        }

        private void Awake() 
        {
            networkObject = GetComponent<NetworkObject>();    
        }
    }
}
