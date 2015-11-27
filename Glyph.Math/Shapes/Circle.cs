using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Circle : ICircle
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2 center, float radius)
            : this()
        {
            Center = center;
            Radius = radius;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return (point - Center).Length() <= Radius;
        }

        public bool Intersects(IRectangle rectangle)
        {
            return IntersectionUtils.RectangleAndCircle(rectangle, this);
        }

        public bool Intersects(ICircle circle)
        {
            return IntersectionUtils.TwoCircles(this, circle);
        }
    }
}