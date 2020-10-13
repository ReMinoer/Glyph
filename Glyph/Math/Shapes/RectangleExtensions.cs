using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    static public class RectangleExtensions
    {
        static public Vector2 NormalizedCoordinates(this Vector2 point, TopLeftRectangle referential)
        {
            return (point - referential.Position) / referential.Size;
        }

        static public TopLeftRectangle NormalizedCoordinates(this TopLeftRectangle rectangle, TopLeftRectangle referential)
        {
            return new TopLeftRectangle((rectangle.Position - referential.Position) / referential.Size, rectangle.Size / referential.Size);
        }

        static public Vector2 Scale(this Vector2 point, TopLeftRectangle rectangle)
        {
            return point * rectangle.Size + rectangle.Position;
        }

        static public TopLeftRectangle Scale(this TopLeftRectangle rectangle, TopLeftRectangle referential)
        {
            return new TopLeftRectangle(rectangle.Position * referential.Size + referential.Position, rectangle.Size * referential.Size);
        }

        static public Vector2 Rescale(this Vector2 point, TopLeftRectangle oldRectangle, TopLeftRectangle newRectangle)
        {
            return point.NormalizedCoordinates(oldRectangle).Scale(newRectangle);
        }

        static public TopLeftRectangle Rescale(this TopLeftRectangle rectangle, TopLeftRectangle oldReferential, TopLeftRectangle newReferential)
        {
            return rectangle.NormalizedCoordinates(oldReferential).Scale(newReferential);
        }

        static public CenteredRectangle Scale(this CenteredRectangle rectangle, float scale)
        {
            return new CenteredRectangle(rectangle.Center, rectangle.Size * scale);
        }

        static public Vector2 ClampToRectangle(this Vector2 point, TopLeftRectangle rectangle)
        {
            if (point.X < rectangle.Left)
                point.X = rectangle.Left;
            if (point.X > rectangle.Right)
                point.X = rectangle.Right;
            if (point.Y < rectangle.Top)
                point.Y = rectangle.Top;
            if (point.Y > rectangle.Bottom)
                point.Y = rectangle.Bottom;

            return point;
        }

        static public Point ClampToRectangle(this Point point, Rectangle rectangle)
        {
            if (point.X < rectangle.Left)
                point.X = rectangle.Left;
            if (point.X > rectangle.Right)
                point.X = rectangle.Right;
            if (point.Y < rectangle.Top)
                point.Y = rectangle.Top;
            if (point.Y > rectangle.Bottom)
                point.Y = rectangle.Bottom;

            return point;
        }

        static public TopLeftRectangle ClampToRectangle(this TopLeftRectangle inner, TopLeftRectangle outer)
        {
            if (inner.Left < outer.Left)
            {
                inner.Width -= outer.Left - inner.Left;
                inner.Left = outer.Left;
            }
            if (inner.Right > outer.Right)
                inner.Width -= inner.Right - outer.Right;

            if (inner.Top < outer.Top)
            {
                inner.Height -= outer.Top - inner.Top;
                inner.Top = outer.Top;
            }
            if (inner.Bottom > outer.Bottom)
                inner.Height -= inner.Bottom - outer.Bottom;

            return inner;
        }

        static public Rectangle ClampToRectangle(this Rectangle inner, Rectangle outer)
        {
            if (inner.Left < outer.Left)
            {
                inner.Width -= outer.Left - inner.Left;
                inner.X = outer.Y;
            }
            if (inner.Right > outer.Right)
                inner.Width -= inner.Right - outer.Right;

            if (inner.Top < outer.Top)
            {
                inner.Height -= outer.Top - inner.Top;
                inner.Y = outer.Y;
            }
            if (inner.Bottom > outer.Bottom)
                inner.Height -= inner.Bottom - outer.Bottom;

            return inner;
        }

        static public TopLeftRectangle EncaseRectangle(this TopLeftRectangle inner, TopLeftRectangle outer)
        {
            if (inner.Left < outer.Left)
                inner.Left += outer.Left - inner.Left;
            else if (inner.Right > outer.Right)
                inner.Left -= inner.Right - outer.Right;

            if (inner.Top < outer.Top)
                inner.Top += outer.Top - inner.Top;
            else if (inner.Bottom > outer.Bottom)
                inner.Top -= inner.Bottom - outer.Bottom;

            return inner;
        }

        static public Rectangle EncaseRectangle(this Rectangle inner, Rectangle outer)
        {
            if (inner.Left < outer.Left)
                inner.X += outer.Left - inner.Left;
            else if (inner.Right > outer.Right)
                inner.X -= inner.Right - outer.Right;

            if (inner.Top < outer.Top)
                inner.Y += outer.Top - inner.Top;
            else if (inner.Bottom > outer.Bottom)
                inner.Y -= inner.Bottom - outer.Bottom;

            return inner;
        }
    }
}