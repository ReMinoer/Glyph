using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.UI
{
    public interface IFrame : IControl
    {
        Vector2 Size { get; set; }
        TopLeftRectangle Bounds { get; }
    }
}