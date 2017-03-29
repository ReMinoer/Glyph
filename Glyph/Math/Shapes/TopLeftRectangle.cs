using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct TopLeftRectangle : IRectangle
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2 Position
        {
            get { return new Vector2(Left, Top); }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 P1 => new Vector2(Right, Top);
        public Vector2 P2 => new Vector2(Right, Bottom);
        public Vector2 P3 => new Vector2(Left, Bottom);

        public Vector2 Center
        {
            get { return Position + Size / 2; }
            set
            {
                Left = value.X - Width / 2;
                Top = value.Y - Height / 2;
            }
        }

        public float Right
        {
            get { return Left + Width; }
            set { Width = value - Left; }
        }

        public float Bottom
        {
            get { return Top + Height; }
            set { Height = value - Top; }
        }

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

        public TopLeftRectangle(Vector2 origin, Vector2 size)
            : this(origin.X, origin.Y, size.X, size.Y)
        {
        }

        public TopLeftRectangle(float x, float y, float width, float height)
            : this()
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
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
            return $"Origin: {Position} - Size: {Size}";
        }
    }
}