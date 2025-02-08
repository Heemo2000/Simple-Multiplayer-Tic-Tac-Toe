using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class SquashStretchEffect : MonoBehaviour
    {
        
        [Range(0.0f, 1.0f),SerializeField]private float duration = 1.0f;
        [SerializeField]private bool affectX;
        [SerializeField]private AnimationCurve xScaleCurve;
        [SerializeField]private bool affectY;
        [SerializeField]private AnimationCurve yScaleCurve;
        [SerializeField]private Vector2 maxScale = new Vector2(2.0f, 2.0f);

        public UnityEvent OnEndEffect;
        private RectTransform rect;

        private Coroutine _effectCoroutine;
        public void PlayEffect()
        {
            if(_effectCoroutine == null)
            {
                _effectCoroutine = StartCoroutine(PlayEffectCoroutine());
            }
        }

        private IEnumerator PlayEffectCoroutine()
        {
            float elapsedTime = 0.0f;
            Vector2 initialScale = rect.localScale;
            Vector2 modifiedScale = initialScale;

            while(elapsedTime < duration)
            {
                float curvePosition = elapsedTime/duration;
                Vector2 curveValue = new Vector2(xScaleCurve.Evaluate(curvePosition),
                                                 yScaleCurve.Evaluate(curvePosition));

                Vector2 remappedValue = initialScale + new Vector2(curveValue.x * (maxScale.x - initialScale.x),
                                                                   curveValue.y * (maxScale.y - initialScale.y)); 
                
                float minThreshold = 0.001f;
                if(Mathf.Abs(remappedValue.x) < minThreshold)
                {
                    remappedValue.x = minThreshold;
                }

                if(Mathf.Abs(remappedValue.y) < minThreshold)
                {
                    remappedValue.y = minThreshold;
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

                rect.localScale = modifiedScale;
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            _effectCoroutine = null;
            rect.localScale = initialScale;
            OnEndEffect?.Invoke();
        }
        
        private void Awake() 
        {
            rect = GetComponent<RectTransform>();
        }

        

        
    }
}
