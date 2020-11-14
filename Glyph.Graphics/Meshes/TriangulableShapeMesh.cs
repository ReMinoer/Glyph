using System.Collections.Generic;
using System.Linq;
using Diese.Collections.ReadOnly;
using Glyph.Graphics.Meshes.Base;
using Glyph.Graphics.Meshes.Utils;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes
{
    public class TriangulableShapeMesh<TTriangulableShape> : MeshBase
        where TTriangulableShape : ITriangulableShape
    {
        private TTriangulableShape _shape;
        private ReadOnlyList<Vector2> _readOnlyVertices;
        private ReadOnlyList<int> _readOnlyIndexes;
        private ReadOnlyList<Vector2> _readOnlyTextureCoordinates;

        public TTriangulableShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                _readOnlyVertices = new ReadOnlyList<Vector2>((_shape?.Vertices ?? Enumerable.Empty<Vector2>()).ToArray());
                _readOnlyIndexes = _shape?.TriangulationIndices != null ? new ReadOnlyList<int>(_shape.TriangulationIndices.Cast<int>().ToArray()) : null;
                _readOnlyTextureCoordinates = new ReadOnlyList<Vector2>(MeshHelpers.GetOrthographicTextureCoordinates(this).ToArray());
            }
        }

        protected override IReadOnlyList<Vector2> ReadOnlyVertices => _readOnlyVertices;
        protected override IReadOnlyList<int> ReadOnlyIndices => _readOnlyIndexes;
        protected override IReadOnlyList<Vector2> ReadOnlyTextureCoordinates => _readOnlyTextureCoordinates;

        public override int VertexCount => Shape?.VertexCount ?? 0;
        public override int IndexCount => Shape.TriangulationIndices == null ? 0 : Shape.StripTriangulation ? Shape.TriangleCount + 2 : Shape.TriangleCount * 3;
        public override PrimitiveType Type => Shape != null && Shape.StripTriangulation ? PrimitiveType.TriangleStrip : PrimitiveType.TriangleList;

        public Color Color { get; set; }
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
    }
}