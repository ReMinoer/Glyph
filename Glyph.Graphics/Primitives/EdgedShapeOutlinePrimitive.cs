using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class EdgedShapeOutlinePrimitive<TEdgedShape> : OutlinePrimitiveBase
        where TEdgedShape : IEdgedShape
    {
        private TEdgedShape _shape;
        public TEdgedShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                DirtyVertices();
                DirtyIndices();
            }
        }

        protected override PrimitiveType PrimitiveType => PrimitiveType.LineStrip;

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

        protected override IEnumerable<Vector2> GetRefreshedVertices()
        {
            using (IEnumerator<Segment> enumerator = _shape.Edges.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    yield break;

                yield return enumerator.Current.P0;
                yield return enumerator.Current.P1;
                while (enumerator.MoveNext())
                    yield return enumerator.Current.P1;
            }
        }
        
        protected override int GetRefreshedVertexCount() => _shape.VertexCount + 1;

        protected override IEnumerable<ushort> GetRefreshedIndices() => null;
        protected override int GetRefreshedIndexCount() => 0;
    }
}