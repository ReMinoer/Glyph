using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class RectangleExtension
    {
        static public Point Size(this Rectangle value)
        {
            return new Point(value.Width, value.Height);
        }

        static public Point Origin(this Rectangle value)
        {
            return new Point(value.X, value.Y);
        }

        static public Point P1(this Rectangle value)
        {
            return new Point(value.Right, value.Y);
        }

        static public Point P2(this Rectangle value)
        {
            return new Point(value.X, value.Bottom);
        }

        static public Point P3(this Rectangle value)
        {
            return new Point(value.Right, value.Bottom);
        }

        static public Rectangle Padding(this Rectangle value, int amount)
        {
            return Padding(value, amount, amount, amount, amount);
        }

        static public Rectangle Padding(this Rectangle value, int top, int right, int bottom, int left)
        {
            return new Rectangle
            {
                X = value.X - left,
                Y = value.Y - top,
                Width = value.Width + left + right,
                Height = value.Height + top + bottom
            };
        }

        static public bool Impact(this Rectangle value, Rectangle other)
        {
            return value.Bottom >= other.Top && value.Top <= other.Bottom && value.Right >= other.Left
                   && value.Left <= other.Right;
        }
    }
}