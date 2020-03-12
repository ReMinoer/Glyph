using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class TriangulableShapePrimitive<TTriangulableShape> : PrimitiveBase
        where TTriangulableShape : ITriangulableShape
    {
        private TTriangulableShape _shape;
        public TTriangulableShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                DirtyVertices();
                DirtyIndices();
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                DirtyColors();
            }
        }

        protected override PrimitiveType PrimitiveType => Shape != null && Shape.StripTriangulation ? PrimitiveType.TriangleStrip : PrimitiveType.TriangleList;

        public TriangulableShapePrimitive()
        {
        }

        public TriangulableShapePrimitive(TTriangulableShape shape)
        {
            _shape = shape;
        }

        public TriangulableShapePrimitive(Color color, TTriangulableShape shape)
            : this(shape)
        {
            Color = color;
        }

        protected override IEnumerable<Vector2> GetRefreshedVertices() => _shape.Vertices;
        protected override IEnumerable<ushort> GetRefreshedIndices() => _shape.TriangulationIndices;
        protected override Color GetRefreshedColor(int vertexIndex) => Color;
        protected override int GetRefreshedVertexCount() => _shape.VertexCount;

        protected override int GetRefreshedIndexCount()
        {
            if (_shape.TriangulationIndices == null)
                return 0;
            
            return _shape.StripTriangulation ? _shape.TriangleCount + 2 : _shape.TriangleCount * 3;
        }
    }
}