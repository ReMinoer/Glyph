using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Circle : IShape
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public TopLeftRectangle BoundingBox => new TopLeftRectangle(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);

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

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return IntersectionUtils.RectangleWithCircle(rectangle, this);
        }

        public bool Intersects(Circle circle)
        {
            return IntersectionUtils.CircleWithCircle(this, circle);
        }

        public override string ToString()
        {
            return $"Center: {Center.X} {Center.Y} - Radius: {Radius}";
        }
    }
}