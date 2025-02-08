using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class UIAnimationHelper
    {
        public static IEnumerator ZoomIn(RectTransform transform, float speed, UnityEvent OnEnd)
        {
            transform.gameObject.SetActive(true);
            float time = 0;
            while (time < 1)
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            transform.localScale = Vector3.one;

            OnEnd?.Invoke();
        }

        public static IEnumerator ZoomOut(RectTransform transform, float speed, UnityEvent OnEnd)
        {
            transform.gameObject.SetActive(true);
            float time = 0;
            while (time < 1)
            {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            transform.localScale = Vector3.zero;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeIn(CanvasGroup canvasGroup, float speed, UnityEvent OnEnd)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            float time = 0;
            while (time < 1)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            canvasGroup.alpha = 1;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeOut(CanvasGroup canvasGroup, float speed, UnityEvent OnEnd)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            float time = 0;
            while (time < 1)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            canvasGroup.alpha = 0;
            OnEnd?.Invoke();
        }

        public static IEnumerator SlideIn(RectTransform transform, Direction direction, float speed, UnityEvent OnEnd)
        {
            transform.gameObject.SetActive(true);
            Vector2 startPosition;
            switch (direction)
            {
                case Direction.Up:
                    startPosition = new Vector2(0, -Screen.height);
                    break;
                case Direction.Right:
                    startPosition = new Vector2(-Screen.width, 0);
                    break;
                case Direction.Down:
                    startPosition = new Vector2(0, Screen.height);
                    break;
                case Direction.Left:
                    startPosition = new Vector2(Screen.width, 0);
                    break;
                default:
                    startPosition = new Vector2(0, -Screen.height);
                    break;
            }

            float time = 0;
            while (time < 1)
            {
                transform.anchoredPosition = Vector2.Lerp(startPosition, Vector2.zero, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            transform.anchoredPosition = Vector2.zero;
            OnEnd?.Invoke();
        }

        public static IEnumerator SlideOut(RectTransform transform, Direction direction, float speed, UnityEvent OnEnd)
        {
            transform.gameObject.SetActive(true);
            Vector2 endPosition;
            switch (direction)
            {
                case Direction.Up:
                    endPosition = new Vector2(0, Screen.height);
                    break;
                case Direction.Right:
                    endPosition = new Vector2(Screen.width, 0);
                    break;
                case Direction.Down:
                    endPosition = new Vector2(0, -Screen.height);
                    break;
                case Direction.Left:
                    endPosition = new Vector2(-Screen.width, 0);
                    break;
                default:
                    endPosition = new Vector2(0, Screen.height);
                    break;
            }

            float time = 0;
            while (time < 1)
            {
                transform.anchoredPosition = Vector2.Lerp(Vector2.zero, endPosition, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            transform.anchoredPosition = endPosition;
            OnEnd?.Invoke();
        }
    }
}
