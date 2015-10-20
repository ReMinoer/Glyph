using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public interface IRectangle : IShape
    {
        Vector2 Origin { get; }
        float Left { get; }
        float Right { get; }
        float Top { get; }
        float Bottom { get; }
        float Width { get; }
        float Height { get; }
        Vector2 Size { get; }
    }
}