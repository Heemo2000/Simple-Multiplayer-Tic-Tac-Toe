using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Gameplay
{
    public class ParticleSystemController : MonoBehaviour
    {
        [SerializeField]private ParticleSystem[] particleSystems;
        
        private Coroutine playCoroutine;
        private ObjectPool<ParticleSystemController> particlePool;

        public ObjectPool<ParticleSystemController> ParticlePool { get => particlePool; set => particlePool = value; }

        public void SetPool(ObjectPool<ParticleSystemController> pool)
        {
            particlePool = pool;
        }
        public void Play()
        {
            /*
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            }
            */

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
            if(particlePool != null)
            {
                particlePool.Release(this);
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
    }
}
