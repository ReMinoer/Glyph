using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct CenteredRectangle : IRectangle
    {
        public Vector2 Center { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2 Origin
        {
            get { return Center - Size / 2; }
        }

        public float Left
        {
            get { return Center.X - Width / 2; }
        }

        public float Right
        {
            get { return Center.X + Size.X / 2; }
        }

        public float Top
        {
            get { return Center.Y - Size.Y / 2; }
        }

        public float Bottom
        {
            get { return Center.Y + Size.Y / 2; }
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

        public CenteredRectangle(Vector2 center, float width, float height)
            : this()
        {
            Center = center;
            Width = width;
            Height = height;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.X > Left && point.X < Right && point.Y > Top && point.Y < Bottom;
        }

        public bool Intersects(IRectangle rectangle)
        {
            return IntersectionUtils.TwoRectangles(this, rectangle);
        }

        public bool Intersects(ICircle circle)
        {
            return IntersectionUtils.RectangleAndCircle(this, circle);
        }
    }
}