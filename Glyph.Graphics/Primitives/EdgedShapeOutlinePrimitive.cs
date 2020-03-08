using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class EdgedShapeOutlinePrimitive : PrimitiveBase
    {
        private readonly IEdgedShape _shape;

        public override PrimitiveType PrimitiveType => PrimitiveType.LineStrip;

        public override IEnumerable<Vector2> Vertices
        {
            get
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
        }

        public override IEnumerable<ushort> Indices => null;
        public override int VertexCount => _shape.VertexCount + 1;
        public override int IndexCount => 0;
        public override sealed Color[] Colors { get; set; }

        public EdgedShapeOutlinePrimitive(IEdgedShape shape)
        {
            _shape = shape;
        }

        public EdgedShapeOutlinePrimitive(Color color, IEdgedShape shape)
            : this(shape)
        {
            Colors = new[] { color };
        }
    }
}