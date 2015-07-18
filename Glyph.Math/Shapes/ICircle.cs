using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public interface ICircle : IShape
    {
        Vector2 Center { get; }
        float Radius { get; }
    }
}