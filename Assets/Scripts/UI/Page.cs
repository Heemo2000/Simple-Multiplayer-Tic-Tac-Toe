using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class Page : MonoBehaviour
    {
        [SerializeField]
        private float animationSpeed = 1f;
        public bool exitOnNewPagePush = false;
        [SerializeField]
        private EntryMode entryMode = EntryMode.Slide;
        [SerializeField]
        private Direction entryDirection = Direction.Left;
        [SerializeField]
        private EntryMode exitMode = EntryMode.Slide;
        [SerializeField]
        private Direction exitDirection = Direction.Left;
        [SerializeField]
        private UnityEvent OnPushAction;
        [SerializeField]
        private UnityEvent OnPostPushAction;
        [SerializeField]
        private UnityEvent OnPrePopAction;
        [SerializeField]
        private UnityEvent OnPostPopAction;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Coroutine _animationCoroutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Enter()
        {
            OnPushAction?.Invoke();

            gameObject.SetActive(true);
            switch(entryMode)
            {
                case EntryMode.Slide:
                    SlideIn();
                    break;
                case EntryMode.Zoom:
                    ZoomIn();
                    break;
                case EntryMode.Fade:
                    FadeIn();
                    break;
            }
        }

        public void Exit()
        {
	    	OnPrePopAction?.Invoke();
            gameObject.SetActive(true);
            switch (exitMode)
            {
                case EntryMode.Slide:
                    SlideOut();
                    break;
                case EntryMode.Zoom:
                    ZoomOut();
                    break;
                case EntryMode.Fade:
                    FadeOut();
                    break;
            }
        }

        private void SlideIn()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.SlideIn(_rectTransform, entryDirection, animationSpeed, OnPostPushAction));

        }

        private void SlideOut()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.SlideOut(_rectTransform, exitDirection, animationSpeed, OnPostPopAction));

            
        }

        private void ZoomIn()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.ZoomIn(_rectTransform, animationSpeed, OnPostPushAction));

            
        }

        private void ZoomOut()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.ZoomOut(_rectTransform, animationSpeed, OnPostPopAction));

            
        }

        private void FadeIn()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.FadeIn(_canvasGroup, animationSpeed, OnPostPushAction));

            
        }

        private void FadeOut()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.FadeOut(_canvasGroup, animationSpeed, OnPostPopAction));


        }
    }
}
