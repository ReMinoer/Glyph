using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface IArea
    {
        bool IsVoid { get; }
        TopLeftRectangle BoundingBox { get; }
        bool ContainsPoint(Vector2 point);
        bool Intersects(Segment segment);
        bool Intersects<T>(T edgedShape) where T : IEdgedShape;
        bool Intersects(Circle circle);
    }
}