using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Circle : IMovableShape
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public TopLeftRectangle BoundingBox
        {
            get
            {
                float size = Radius + Radius;
                return new TopLeftRectangle(Center.X - Radius, Center.Y - Radius, size, size);
            }
        }

        public bool IsVoid => Radius.EqualsZero();

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

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);

        public override string ToString()
        {
            return $"Center: {Center.X} {Center.Y} - Radius: {Radius}";
        }
    }
}