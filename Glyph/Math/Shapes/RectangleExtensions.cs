using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    static public class RectangleExtensions
    {
        static public Vector2 NormalizedCoordinates(this Vector2 point, TopLeftRectangle referential)
        {
            Vector2 result = (point - referential.Position) / referential.Size;
            if (float.IsNaN(result.X))
                result = result.SetX(0);
            if (float.IsNaN(result.Y))
                result = result.SetY(0);

            return result;
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

        static public TopLeftRectangle Inflate(this TopLeftRectangle rectangle, Vector2 amount) => Inflate((CenteredRectangle)rectangle, amount.X, amount.Y);
        static public TopLeftRectangle Inflate(this TopLeftRectangle rectangle, float widthAmount, float heightAmount) => Inflate((CenteredRectangle)rectangle, widthAmount, heightAmount);
        static public CenteredRectangle Inflate(this CenteredRectangle rectangle, Vector2 amount) => Inflate(rectangle, amount.X, amount.Y);
        static public CenteredRectangle Inflate(this CenteredRectangle rectangle, float widthAmount, float heightAmount)
        {
            float width = rectangle.Width + widthAmount;
            if (width < 0)
                width = 0;

            float height = rectangle.Height + heightAmount;
            if (height < 0)
                height = 0;

            return new CenteredRectangle(rectangle.Center, width, height);
        }
    }
}