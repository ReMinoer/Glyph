using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    static public class PrimitiveExtension
    {
        static public LinePrimitive ToPrimitive(this Segment segment, params Color[] colors)
        {
            return new LinePrimitive(segment.Vertices.ToArray()) { Colors = colors };
        }

        static public IndexedShapePrimitive ToPrimitive(this ITriangulableShape triangulableShape, params Color[] colors)
        {
            return new IndexedShapePrimitive(triangulableShape) { Colors = colors };
        }

        static public EdgedShapeOutlinePrimitive ToOutlinePrimitive(this IEdgedShape edgedShape, params Color[] colors)
        {
            return new EdgedShapeOutlinePrimitive(edgedShape) { Colors = colors };
        }

        static public CirclePrimitive ToPrimitive(this Circle circle, Color color, float thickness = float.MaxValue)
        {
            return new CirclePrimitive(color, circle.Center, circle.Radius, thickness);
        }

        static public CircleOutlinePrimitive ToOutlinePrimitive(this Circle circle, Color color)
        {
            return new CircleOutlinePrimitive(color, circle.Center, circle.Radius);
        }
    }
}