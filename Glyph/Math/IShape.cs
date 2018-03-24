using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface IShape : IArea
    {
        Vector2 Center { get; }
    }

    public interface IEdgedShape : IShape
    {
        IEnumerable<Vector2> Vertices { get; }
        IEnumerable<Segment> Edges { get; }
    }

    public interface ITriangledShape : IEdgedShape
    {
        IEnumerable<Triangle> Triangles { get; }
    }

    public interface IMovableShape : IShape
    {
        new Vector2 Center { get; set; }
    }
}