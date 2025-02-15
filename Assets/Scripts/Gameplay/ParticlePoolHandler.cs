using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using UnityEngine.Pool;
using Unity.Netcode;

namespace Game.Gameplay
{
    public class ParticlePoolHandler : NetworkSingelton<ParticlePoolHandler>
    {
        [SerializeField]private ParticleSystemController[] effectPrefabs;
        [Min(10)]
        [SerializeField]private int maxPoolSize = 10;
        private ObjectPool<ParticleSystemController> particlePool;

        public void PlayParticleEffect(Vector3 position)
        {
            var particle = particlePool.Get();
            particle.transform.position = position;
            particle.transform.GetComponent<NetworkObject>().Spawn(true);
            particle.Play();
        }
        protected override void InternalInit()
        {
            
        }

        protected override void InternalOnStart()
        {
            if(particlePool == null)
            {
                Random.InitState((int)System.DateTime.Now.Ticks);
                particlePool = new ObjectPool<ParticleSystemController>(CreateParticle, 
                                                                        OnParticleGet, 
                                                                        OnParticleRelease, 
                                                                        OnParticleDestroy, 
                                                                        true,
                                                                        maxPoolSize,
                                                                        maxPoolSize);
                Debug.Log("Particle pool created");
            }
            else
            {
                Debug.LogWarning("Particle pool already created!");
            }    
        }

        protected override void InternalOnDestroy()
        {
            
        }

        private ParticleSystemController CreateParticle()
        {
            int randomParticleIndex = Random.Range(0, effectPrefabs.Length);
            var particle = Instantiate(effectPrefabs[randomParticleIndex], transform);
            particle.transform.rotation = Quaternion.identity;
            particle.gameObject.SetActive(false);
            return particle;
        }

        private void OnParticleGet(ParticleSystemController particle)
        {
            particle.gameObject.SetActive(true);
        }

        private void OnParticleRelease(ParticleSystemController particle)
        {
            particle.gameObject.SetActive(false);
        }

        private void OnParticleDestroy(ParticleSystemController particle)
        {
            Destroy(particle.gameObject);
        }
    }
}
