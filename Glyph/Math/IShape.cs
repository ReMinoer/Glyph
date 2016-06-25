using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface IShape : IArea
    {
        Vector2 Center { get; set; }
        bool Intersects(IRectangle rectangle);
        bool Intersects(ICircle circle);
    }
}