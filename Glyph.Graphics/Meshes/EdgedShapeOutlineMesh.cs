using System.Collections.Generic;
using System.Linq;
using Diese.Collections.ReadOnly;
using Glyph.Graphics.Meshes.Base;
using Glyph.Graphics.TextureMappers;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes
{
    public class EdgedShapeOutlineMesh<TEdgedShape> : LineMeshBase
        where TEdgedShape : IEdgedShape
    {
        private TEdgedShape _shape;
        private ReadOnlyList<Vector2> _readOnlyVertices;
        private ReadOnlyList<Vector2> _readOnlyTextureCoordinates;

        public TEdgedShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;

                Vector2[] vertices = GetVertices().ToArray();
                _readOnlyVertices = new ReadOnlyList<Vector2>(vertices);

                Vector2[] textureCoordinates = NormalizedTextureMapper.Instance.GetVertexTextureCoordinates(vertices);
                _readOnlyTextureCoordinates = new ReadOnlyList<Vector2>(textureCoordinates);
            }
        }

        protected override IReadOnlyList<Vector2> ReadOnlyVertices => _readOnlyVertices;
        protected override IReadOnlyList<Vector2> ReadOnlyTextureCoordinates => _readOnlyTextureCoordinates;
        protected override IReadOnlyList<int> ReadOnlyIndices => null;
        public override int VertexCount => Shape.VertexCount + 1;

        protected override bool IsStrip => true;

        public EdgedShapeOutlineMesh()
        {
        }

        public EdgedShapeOutlineMesh(TEdgedShape shape)
        {
            Shape = shape;
        }

        public EdgedShapeOutlineMesh(Color color, TEdgedShape shape)
            : this(shape)
        {
            Colors = new[] { color };
        }

        private IEnumerable<Vector2> GetVertices()
        {
            if (Shape == null)
                yield break;

            using (IEnumerator<Segment> enumerator = Shape.Edges.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    yield break;

                yield return enumerator.Current.P0;
                yield return enumerator.Current.P1;
                while (enumerator.MoveNext())
                    yield return enumerator.Current.P1;
            }
        }
    }
}