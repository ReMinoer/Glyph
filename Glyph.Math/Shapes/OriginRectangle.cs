using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public class OriginRectangle : GlyphComponent, IRectangle
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Vector2 Origin
        {
            get { return new Vector2(Left, Top); }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 Center
        {
            get { return Origin + Size / 2; }
            set { Origin = value - Size / 2; }
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

        public OriginRectangle()
            : this(0, 0, 0, 0)
        {
        }

        public OriginRectangle(float x, float y, float width, float height)
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

        public bool Intersects(IRectangle collider)
        {
            return IntersectionUtils.TwoRectangles(this, collider);
        }

        public bool Intersects(ICircle collider)
        {
            return IntersectionUtils.RectangleAndCircle(this, collider);
        }
    }
}