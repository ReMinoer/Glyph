using System.Collections.Generic;
using Glyph.Math;

namespace Glyph.Space
{
    public interface IPartitioner : IEdgedShape
    {
        int Capacity { get; }
        IEnumerable<IPartitioner> Subdivide();
    }
}