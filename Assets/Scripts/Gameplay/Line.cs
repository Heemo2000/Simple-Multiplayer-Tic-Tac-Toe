using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public struct Line
    {
        private List<Vector2Int> points;
        private Vector2Int centre;

        public List<Vector2Int> Points { get => points; }
        public Vector2Int Centre { get => centre; }
        public Line(List<Vector2Int> points, Vector2Int centre)
        {
            this.points = points;
            this.centre = centre;
        }
    }
}
