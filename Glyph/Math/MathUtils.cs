using System.Collections.Generic;
using System.Linq;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    static public class MathUtils
    {
        static public bool FloatEquals(float a, float b)
        {
            return System.Math.Abs(a - b) < float.Epsilon;
        }

        static public TopLeftRectangle GetBoundingBox(params Vector2[] points)
        {
            return GetBoundingBox(points.AsEnumerable());
        }

        static public TopLeftRectangle GetBoundingBox(IEnumerable<Vector2> points)
        {
            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;

            foreach (Vector2 point in points)
            {
                if (point.X < left)
                    left = point.X;
                if (point.X > right)
                    right = point.X;
                if (point.Y < top)
                    top = point.Y;
                if (point.Y > bottom)
                    bottom = point.Y;
            }

            return new TopLeftRectangle(left, top, right - left, bottom - top);
        }

        static public TopLeftRectangle GetBoundingBox(params TopLeftRectangle[] rectangles)
        {
            return GetBoundingBox(rectangles.AsEnumerable());
        }

        static public TopLeftRectangle GetBoundingBox(IEnumerable<TopLeftRectangle> rectangles)
        {
            TopLeftRectangle[] topLeftRectangles = rectangles as TopLeftRectangle[] ?? rectangles.ToArray();
            if (topLeftRectangles.Length == 0)
                return TopLeftRectangle.Void;

            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;

            foreach (TopLeftRectangle rectangle in topLeftRectangles)
            {
                if (rectangle.Left < left)
                    left = rectangle.Left;
                if (rectangle.Right > right)
                    right = rectangle.Right;
                if (rectangle.Top < top)
                    top = rectangle.Top;
                if (rectangle.Bottom > bottom)
                    bottom = rectangle.Bottom;
            }

            return new TopLeftRectangle(left, top, right - left, bottom - top);
        }

        static public TopLeftRectangle GetBoundingBox(params IArea[] areas)
        {
            return GetBoundingBox(areas.Select(x => x.BoundingBox));
        }

        static public TopLeftRectangle GetBoundingBox(IEnumerable<IArea> areas)
        {
            return GetBoundingBox(areas.Select(x => x.BoundingBox));
        }

        static public Vector2 ClampToRectangle(Vector2 point, TopLeftRectangle rectangle)
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

        static public TopLeftRectangle ClampToRectangle(TopLeftRectangle inner, TopLeftRectangle outer)
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

        static public TopLeftRectangle EncaseRectangle(TopLeftRectangle inner, TopLeftRectangle outer)
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
    }
}