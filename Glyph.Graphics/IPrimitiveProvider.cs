using System.Collections.Generic;

namespace Glyph.Graphics
{
    public interface IPrimitiveProvider
    {
        IEnumerable<IPrimitive> Primitives { get; }
    }
}