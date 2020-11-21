using System.Collections.Generic;
using System.Linq;
using Glyph.Graphics.Meshes.Base;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes
{
    public class TriangulableShapeMesh<TTriangulableShape> : ProceduralMeshBase
        where TTriangulableShape : ITriangulableShape
    {
        private TTriangulableShape _shape;
        public TTriangulableShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                DirtyCaches();
            }
        }

        public override PrimitiveType Type => Shape != null && Shape.StripTriangulation ? PrimitiveType.TriangleStrip : PrimitiveType.TriangleList;

        public Color Color { get; set; } = Color.White;
        protected override Color GetColor(int vertexIndex) => Color;

        public TriangulableShapeMesh()
        {
        }

        public TriangulableShapeMesh(TTriangulableShape shape)
        {
            Shape = shape;
        }

        public TriangulableShapeMesh(Color color, TTriangulableShape shape)
            : this(shape)
        {
            Color = color;
        }

        protected override void RefreshCache(List<Vector2> vertices, List<int> indices)
        {
            vertices.Clear();
            indices.Clear();

            IEnumerable<Vector2> shapeVertices = _shape?.Vertices;
            if (shapeVertices != null)
                vertices.AddRange(_shape.Vertices);

            IEnumerable<ushort> shapeIndices = _shape?.TriangulationIndices;
            if (shapeIndices != null)
                indices.AddRange(shapeIndices.Cast<int>());
        }
    }
}