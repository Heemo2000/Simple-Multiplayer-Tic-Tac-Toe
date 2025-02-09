using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class SquashStretchEffect : MonoBehaviour
    {
        [Range(0.0f, 1.0f),SerializeField]private float duration = 1.0f;
        [SerializeField]private bool affectX;
        [SerializeField]private AnimationCurve xScaleCurve;
        [SerializeField]private bool affectY;
        [SerializeField]private AnimationCurve yScaleCurve;
        [SerializeField]private bool affectZ;
        [SerializeField]private AnimationCurve zScaleCurve;
        [SerializeField]private Vector3 maxScale = new Vector3(2.0f, 2.0f,2.0f);

        public UnityEvent OnStartEffect;
        public UnityEvent OnEndEffect;

        private Coroutine effectCoroutine;
        public void PlayEffect()
        {
            if(effectCoroutine == null)
            {
                effectCoroutine = StartCoroutine(PlayEffectCoroutine());
            }
        }

        private IEnumerator PlayEffectCoroutine()
        {
            float elapsedTime = 0.0f;
            Vector3 initialScale = transform.localScale;
            Vector3 modifiedScale = initialScale;

            OnStartEffect?.Invoke();
            while(elapsedTime < duration)
            {
                float curvePosition = elapsedTime/duration;
                Vector3 curveValue = new Vector3(xScaleCurve.Evaluate(curvePosition),
                                                 yScaleCurve.Evaluate(curvePosition),
                                                 zScaleCurve.Evaluate(curvePosition));

                Vector3 remappedValue = initialScale + new Vector3(curveValue.x * (maxScale.x - initialScale.x),
                                                                   curveValue.y * (maxScale.y - initialScale.y),
                                                                   curveValue.z * (maxScale.z - initialScale.z)); 
                
                float minThreshold = 0.001f;
                if(Mathf.Abs(remappedValue.x) < minThreshold)
                {
                    remappedValue.x = minThreshold;
                }

                if(Mathf.Abs(remappedValue.y) < minThreshold)
                {
                    remappedValue.y = minThreshold;
                }

                if(Math.Abs(remappedValue.z) < minThreshold)
                {
                    remappedValue.z = minThreshold;
                }

                if(affectX)
                {
                    modifiedScale.x = initialScale.x * remappedValue.x;
                }
                else
                {
                    modifiedScale.x = initialScale.x / remappedValue.x;
                }

                if(affectY)
                {
                    modifiedScale.y = initialScale.y * remappedValue.y;
                }
                else
                {
                    modifiedScale.y = initialScale.y / remappedValue.y;
                }

                if(affectZ)
                {
                    modifiedScale.z = initialScale.z * remappedValue.z;
                }
                else
                {
                    modifiedScale.z = initialScale.z / remappedValue.z;
                }

                transform.localScale = modifiedScale;
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            effectCoroutine = null;
            transform.localScale = initialScale;
            OnEndEffect?.Invoke();
        }
    }
}
