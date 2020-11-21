using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes
{
    static public class MeshExtension
    {
        static public LineMesh ToMesh(this Segment segment, params Color[] colors)
        {
            return new LineMesh(segment.Vertices.ToArray()) { Colors = colors };
        }

        static public TriangulableShapeMesh<TTriangulableShape> ToMesh<TTriangulableShape>(this TTriangulableShape triangulableShape)
            where TTriangulableShape : ITriangulableShape
        {
            return new TriangulableShapeMesh<TTriangulableShape>(triangulableShape);
        }

        static public TriangulableShapeMesh<TTriangulableShape> ToMesh<TTriangulableShape>(this TTriangulableShape triangulableShape, Color color)
            where TTriangulableShape : ITriangulableShape
        {
            return new TriangulableShapeMesh<TTriangulableShape>(color, triangulableShape);
        }

        static public EdgedShapeOutlineMesh<TEdgedShape> ToOutlineMesh<TEdgedShape>(this TEdgedShape edgedShape, params Color[] colors)
            where TEdgedShape : IEdgedShape
        {
            return new EdgedShapeOutlineMesh<TEdgedShape>(edgedShape) { Colors = colors };
        }

        static public EllipseMesh ToMesh(this Circle circle, float thickness = float.MaxValue)
        {
            return new EllipseMesh(circle.Center, circle.Radius, thickness: thickness);
        }

        static public EllipseMesh ToMesh(this Circle circle, Color color, float thickness = float.MaxValue)
        {
            return new EllipseMesh(color, circle.Center, circle.Radius, thickness: thickness);
        }

        static public EllipseOutlineMesh ToOutlineMesh(this Circle circle)
        {
            return new EllipseOutlineMesh(circle.Center, circle.Radius);
        }

        static public EllipseOutlineMesh ToOutlineMesh(this Circle circle, Color color)
        {
            return new EllipseOutlineMesh(color, circle.Center, circle.Radius);
        }
    }
}