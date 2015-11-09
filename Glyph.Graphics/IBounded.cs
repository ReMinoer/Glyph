using Glyph.Composition;
using Glyph.Math.Shapes;

namespace Glyph.Graphics
{
    public interface IBounded : IGlyphComponent
    {
        IRectangle Bounds { get; }
    }
}