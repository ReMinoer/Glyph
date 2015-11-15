using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.UI
{
    public interface IBorder : IDraw
    {
        Color Color { get; set; }
        OriginRectangle Bounds { get; }
    }
}