using System.Collections.Generic;
using System.Linq;
using Diese.Collections.ReadOnly;
using Glyph.Graphics.Primitives.Base;
using Glyph.Graphics.Primitives.Utils;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    public class EdgedShapeOutlinePrimitive<TEdgedShape> : LinePrimitiveBase
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
                _readOnlyVertices = new ReadOnlyList<Vector2>(GetVertices().ToArray());
                _readOnlyTextureCoordinates = new ReadOnlyList<Vector2>(PrimitiveHelpers.GetOrthographicTextureCoordinates(this).ToArray());
            }
        }

        protected override IReadOnlyList<Vector2> ReadOnlyVertices => _readOnlyVertices;
        protected override IReadOnlyList<Vector2> ReadOnlyTextureCoordinates => _readOnlyTextureCoordinates;
        protected override IReadOnlyList<int> ReadOnlyIndices => null;
        public override int VertexCount => Shape.VertexCount + 1;

        protected override bool IsStrip => true;

        public EdgedShapeOutlinePrimitive()
        {
        }

        public EdgedShapeOutlinePrimitive(TEdgedShape shape)
        {
            Shape = shape;
        }

        public EdgedShapeOutlinePrimitive(Color color, TEdgedShape shape)
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