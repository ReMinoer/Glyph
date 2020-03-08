using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public interface IRectangle : ITriangulableShape
    {
        Vector2 Position { get; }
        Vector2 P1 { get; }
        Vector2 P2 { get; }
        Vector2 P3 { get; }
        float Left { get; }
        float Right { get; }
        float Top { get; }
        float Bottom { get; }
        float Width { get; }
        float Height { get; }
        Vector2 Size { get; }
    }
}