using System.Collections.Generic;
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

        static public TriangulableShapePrimitive<TTriangulableShape> ToPrimitive<TTriangulableShape>(this TTriangulableShape triangulableShape, Color color)
            where TTriangulableShape : ITriangulableShape
        {
            return new TriangulableShapePrimitive<TTriangulableShape>(color, triangulableShape);
        }

        static public EdgedShapeOutlinePrimitive<TEdgedShape> ToOutlinePrimitive<TEdgedShape>(this TEdgedShape edgedShape, params Color[] colors)
            where TEdgedShape : IEdgedShape
        {
            return new EdgedShapeOutlinePrimitive<TEdgedShape>(edgedShape) { Colors = colors };
        }

        static public EllipsePrimitive ToPrimitive(this Circle circle, Color color, float thickness = float.MaxValue)
        {
            return new EllipsePrimitive(color, circle.Center, circle.Radius, thickness: thickness);
        }

        static public EllipseOutlinePrimitive ToOutlinePrimitive(this Circle circle, Color color)
        {
            return new EllipseOutlinePrimitive(color, circle.Center, circle.Radius);
        }
    }
}