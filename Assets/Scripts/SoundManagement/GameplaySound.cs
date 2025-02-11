using System.Collections;
using System.Collections.Generic;
using Game.Gameplay;
using UnityEngine;

namespace Game.SoundManagement
{
    public class GameplaySound : MonoBehaviour
    {
        [SerializeField]private SoundData musicData;
        [SerializeField]private SoundData placingObjectSFXData;
        [SerializeField]private SoundData winSFXData;
        [SerializeField]private SoundData loseSFXData;


        private void OnGameStarted()
        {
            SoundManager.Instance.Play(musicData, transform.position);
        }

        private void OnPlacedObject()
        {
            SoundManager.Instance.Play(placingObjectSFXData, transform.position);
        }

        private void OnGameWin(Line line, MarkType winPlayerType)
        {
            if(winPlayerType == GameManager.Instance.LocalPlayerType)
            {
                SoundManager.Instance.Play(winSFXData, transform.position);
            }
            else
            {
                SoundManager.Instance.Play(loseSFXData, transform.position);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnGameStarted += OnGameStarted;
            GameManager.Instance.OnPlacedObject += OnPlacedObject;
            GameManager.Instance.OnGameWin += OnGameWin;
        }
    }
}
