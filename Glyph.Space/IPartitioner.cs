using System.Collections.Generic;
using Glyph.Math;

namespace Glyph.Space
{
    public interface IPartitioner : IShape
    {
        int Capacity { get; }
        bool Intersects(IShape shape);
        IEnumerable<IPartitioner> Subdivide();
    }
}