using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Gameplay
{
    public class GridPosition : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]private int x = 0;
        [SerializeField]private int y = 0;

        public int X { get => x; }
        public int Y { get => y; }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("X: " + x + ",Y: " + y);
            GameManager.Instance.ClickedOnGridPositionServerRpc(x, y, GameManager.Instance.LocalPlayerType);
        }

        
    }
}
