using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField]private Sprite circleSprite;
        [SerializeField]private Sprite crossSprite;
        [SerializeField]private SpriteRenderer graphics;
        [SerializeField]private SpriteSquashyStretchEffect stretchEffect;        
        
        private Vector2Int inGridPosition = Vector2Int.zero;

        public Vector2Int InGridPosition { get => inGridPosition; set => inGridPosition = value; }

        public void SetMarkType(MarkType markType)
        {
            switch(markType)
            {
                case MarkType.None:
                                   graphics.sprite = null;
                                   break;
                
                case MarkType.O:
                                   graphics.sprite = circleSprite;
                                   stretchEffect.PlayEffect();
                                   break;

                case MarkType.X:
                                   graphics.sprite = crossSprite;
                                   stretchEffect.PlayEffect();
                                   break;
            }
        }
    }
}
