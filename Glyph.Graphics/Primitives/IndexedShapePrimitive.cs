using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class IndexedShapePrimitive : PrimitiveBase
    {
        private readonly ITriangulableShape _shape;

        public override PrimitiveType PrimitiveType => _shape.StripTriangulation ? PrimitiveType.TriangleStrip : PrimitiveType.TriangleList;

        public override IEnumerable<Vector2> Vertices => _shape.Vertices;
        public override IEnumerable<ushort> Indices => _shape.TriangulationIndices;
        public override int VertexCount => _shape.VertexCount;
        public override int IndexCount => _shape.TriangulationIndices != null ? (_shape.StripTriangulation ? _shape.TriangleCount + 2 : _shape.TriangleCount * 3) : 0;
        public override sealed Color[] Colors { get; set; }

        public IndexedShapePrimitive(ITriangulableShape shape)
        {
            _shape = shape;
        }

        public IndexedShapePrimitive(Color color, ITriangulableShape shape)
            : this(shape)
        {
            Colors = new[] { color };
        }
    }
}