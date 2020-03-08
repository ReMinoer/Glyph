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
        int VertexCount { get; }
        int EdgeCount { get; }
        IEnumerable<Vector2> Vertices { get; }
        IEnumerable<Segment> Edges { get; }
    }

    public interface ITriangulableShape : IEdgedShape
    {
        int TriangleCount { get; }
        IEnumerable<ushort> TriangulationIndices { get; }
        Vector2 GetIndexedVertex(ushort index);
        bool StripTriangulation { get; }
    }

    public interface IMovableShape : IShape
    {
        new Vector2 Center { get; set; }
    }
}