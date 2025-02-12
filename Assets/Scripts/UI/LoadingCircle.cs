using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class LoadingCircle : MonoBehaviour
    {
        [Min(1.0f)]
        [SerializeField]private float initialRotateSpeed = 10.0f;
        [Min(0.1f)]
        [SerializeField]private float acceleration = 2.0f;

        [Min(0.0f)]
        [SerializeField]private float maxRotateTime = 3.0f;

        private float finalRotateSpeed = 0.0f;
        private float currentTime = 0.0f;

        private bool shouldAccelerate = true;

        private RectTransform rectTransform;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
        }
        private void Start() {
            shouldAccelerate = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(shouldAccelerate)
            {
                currentTime += Time.deltaTime;
                if(currentTime >= maxRotateTime)
                {
                    currentTime = maxRotateTime;
                    shouldAccelerate = false;
                }
            }
            else
            {
                currentTime -= Time.deltaTime;
                if(currentTime <= 0.0f)
                {
                    currentTime = 0.0f;
                    shouldAccelerate = true;
                }
            }


            finalRotateSpeed = initialRotateSpeed + acceleration * currentTime;

            Vector3 eulerAngles = rectTransform.eulerAngles;
            eulerAngles.z -= finalRotateSpeed;
            rectTransform.eulerAngles = eulerAngles;
        }
    }
}
