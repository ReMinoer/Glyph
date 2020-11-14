using System.Collections.Generic;
using Glyph.Graphics.Meshes.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes
{
    public class LineMesh : LineProceduralMeshBase
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

        public LineMesh()
        {
        }

        public LineMesh(params Vector2[] points)
        {
            Points = points;
        }

        public LineMesh(Color color, params Vector2[] points)
            : this(points)
        {
            Colors = new[] { color };
        }

        public LineMesh(Color color, bool isLineList, params Vector2[] points)
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