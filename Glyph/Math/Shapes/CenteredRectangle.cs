using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct CenteredRectangle : IRectangle
    {
        public Vector2 Center { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        
        public Vector2 Position => new Vector2(Left, Top);
        public Vector2 P1 => new Vector2(Right, Top);
        public Vector2 P2 => new Vector2(Right, Bottom);
        public Vector2 P3 => new Vector2(Left, Bottom);

        public float Left => Center.X - Width / 2;
        public float Right => Center.X + Width / 2;
        public float Top => Center.Y - Height / 2;
        public float Bottom => Center.Y + Height / 2;

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        TopLeftRectangle IArea.BoundingBox => this;

        static public TopLeftRectangle Void => new TopLeftRectangle();

        public CenteredRectangle(Vector2 center, float width, float height)
            : this()
        {
            Center = center;
            Width = width;
            Height = height;
        }

        public CenteredRectangle(Vector2 center, Vector2 size)
            : this()
        {
            Center = center;
            Size = size;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.X > Left && point.X < Right && point.Y > Top && point.Y < Bottom;
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return IntersectionUtils.RectangleWithRectangle(this, rectangle);
        }

        public bool Intersects(TopLeftRectangle rectangle, out TopLeftRectangle intersection)
        {
            return IntersectionUtils.RectangleWithRectangle(this, rectangle, out intersection);
        }

        public bool Intersects(Circle circle)
        {
            return IntersectionUtils.RectangleWithCircle(this, circle);
        }

        public override string ToString()
        {
            return $"Center: {Center} - Size: {Size}";
        }

        static public implicit operator CenteredRectangle(TopLeftRectangle rectangle)
        {
            return new CenteredRectangle(rectangle.Center, rectangle.Width, rectangle.Height);
        }

        static public implicit operator TopLeftRectangle(CenteredRectangle rectangle)
        {
            return new TopLeftRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
        }
    }
}