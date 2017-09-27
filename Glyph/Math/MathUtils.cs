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
    }
}