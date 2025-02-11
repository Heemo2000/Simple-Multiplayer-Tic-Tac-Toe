using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Gameplay;
namespace Game.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]private Image rightArrow;
        [SerializeField]private Image leftArrow;
        [SerializeField]private Image cross;
        [SerializeField]private Image circle;
        [SerializeField]private TMP_Text crossText;
        [SerializeField]private TMP_Text circleText;
        [SerializeField]private TMP_Text crossWinsText;
        [SerializeField]private TMP_Text circleWinsText;
        

        private void IncreaseWinCount()
        {
            int crossWinsCount = 0;
            int circleWinsCount = 0;
            GameManager.Instance.GetScores(out crossWinsCount, out circleWinsCount);
            SetCrossWinsText(crossWinsCount);
            SetCircleWinsText(circleWinsCount);
        }
        private void SetCrossWinsText(int wins)
        {
            crossWinsText.text = wins.ToString();
        }

        private void SetCircleWinsText(int wins)
        {
            circleWinsText.text = wins.ToString();
        }
        private void Reset()
        {
            rightArrow.gameObject.SetActive(false);
            leftArrow.gameObject.SetActive(false);
            cross.gameObject.SetActive(true);
            circle.gameObject.SetActive(true);
            crossText.gameObject.SetActive(false);
            circleText.gameObject.SetActive(false);
            crossWinsText.gameObject.SetActive(false);
            circleWinsText.gameObject.SetActive(false);
        }

        private void OnGameStarted()
        {
            Debug.Log("Game has started.");
            Reset();
            
            crossWinsText.gameObject.SetActive(true);
            circleWinsText.gameObject.SetActive(true);

            SetCrossWinsText(0);
            SetCircleWinsText(0);

            if(GameManager.Instance.LocalPlayerType == MarkType.Cross)
            {
                crossText.gameObject.SetActive(true);
            }
            else
            {
                circleText.gameObject.SetActive(true);
            }

            UpdateCurrentArrow();
        }

        private void UpdateCurrentArrow()
        {
            Debug.Log("Updating current arrow");
            if(GameManager.Instance.CurrentPlayablePlayerType == MarkType.Cross)
            {
                rightArrow.gameObject.SetActive(true);
                leftArrow.gameObject.SetActive(false);
            }
            else if(GameManager.Instance.CurrentPlayablePlayerType == MarkType.Circle)
            {
                rightArrow.gameObject.SetActive(false);
                leftArrow.gameObject.SetActive(true);
            }
        }

        private void Awake() {
            Reset();
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnGameStarted += OnGameStarted;
            GameManager.Instance.OnCurrentPlayablePlayerChanged += UpdateCurrentArrow;
            GameManager.Instance.OnScoreChanged += IncreaseWinCount; 
        }

        private void OnDestroy() 
        {
            GameManager.Instance.OnGameStarted -= OnGameStarted;
            GameManager.Instance.OnCurrentPlayablePlayerChanged -= UpdateCurrentArrow;
            GameManager.Instance.OnScoreChanged -= IncreaseWinCount; 
        }
    }
}
