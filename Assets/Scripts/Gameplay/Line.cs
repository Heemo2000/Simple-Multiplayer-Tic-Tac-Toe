using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public struct Line
    {
        private List<Vector2Int> points;
        private Vector2Int centre;
        private Orientation orientation;

        public List<Vector2Int> Points { get => points; }
        public Vector2Int Centre { get => centre; }
        public Orientation Orientation { get => orientation; }

        public Line(List<Vector2Int> points, Vector2Int centre, Orientation orientation)
        {
            this.points = points;
            this.centre = centre;
            this.orientation = orientation;
        }
    }
}
