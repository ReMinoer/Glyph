using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public class Circle : ICircle
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return (point - Center).Length() <= Radius;
        }

        public bool Intersects(IRectangle collider)
        {
            return IntersectionUtils.RectangleAndCircle(collider, this);
        }

        public bool Intersects(ICircle collider)
        {
            return IntersectionUtils.TwoCircles(this, collider);
        }
    }
}