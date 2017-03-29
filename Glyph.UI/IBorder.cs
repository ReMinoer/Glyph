using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.UI
{
    public interface IBorder : IControl
    {
        Color Color { get; }
        int Thickness { get; }
        Vector2 Size { get; set; }
        TopLeftRectangle Bounds { get; }
    }
}