using Glyph.Composition;
using Glyph.Math;

namespace Glyph.Core
{
    public interface IBoxedComponent : IGlyphComponent
    {
        IArea Area { get; }
    }
}