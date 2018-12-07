using Glyph.Composition;
using Glyph.Math;

namespace Glyph.Core
{
    public interface ICamera : IGlyphComponent, ITransformation
    {
        ITransformation RenderTransformation { get; }
    }
}