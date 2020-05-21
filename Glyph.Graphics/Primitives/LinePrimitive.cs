using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    public class LinePrimitive : LineProceduralPrimitiveBase
    {
        private Vector2[] _points;
        public Vector2[] Points
        {
            get => _points;
            set
            {
                if (_points == value)
                    return;

                _points = value;
                DirtyVertices();
            }
        }

        public bool IsLineList { get; set; }
        protected override bool IsStrip => !IsLineList;

        public LinePrimitive()
        {
        }

        public LinePrimitive(params Vector2[] points)
        {
            Points = points;
        }

        public LinePrimitive(Color color, params Vector2[] points)
            : this(points)
        {
            Colors = new[] { color };
        }

        public LinePrimitive(Color color, bool isLineList, params Vector2[] points)
            : this(color, points)
        {
            IsLineList = isLineList;
        }

        protected override int GetRefreshedVertexCount() => Points.Length;
        protected override IEnumerable<Vector2> GetRefreshedVertices() => Points;
        protected override int GetRefreshedIndexCount() => 0;
        protected override IEnumerable<int> GetRefreshedIndices() => null;
    }
}