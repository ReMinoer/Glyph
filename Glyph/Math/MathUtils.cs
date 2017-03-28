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

        static public IRectangle GetBoundingBox(params Vector2[] points)
        {
            return GetBoundingBox(points.AsEnumerable());
        }

        static public IRectangle GetBoundingBox(IEnumerable<Vector2> points)
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

            return new OriginRectangle(left, top, right - left, bottom - top);
        }

        static public IRectangle GetBoundingBox(params IRectangle[] rectangles)
        {
            return GetBoundingBox(rectangles.AsEnumerable());
        }

        static public IRectangle GetBoundingBox(IEnumerable<IRectangle> rectangles)
        {
            float left = float.MaxValue;
            float right = float.MinValue;
            float top = float.MaxValue;
            float bottom = float.MinValue;

            foreach (IRectangle rectangle in rectangles)
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

            return new OriginRectangle(left, top, right - left, bottom - top);
        }

        static public IRectangle GetBoundingBox(params IArea[] areas)
        {
            return GetBoundingBox(areas.Select(x => x.BoundingBox));
        }

        static public IRectangle GetBoundingBox(IEnumerable<IArea> areas)
        {
            return GetBoundingBox(areas.Select(x => x.BoundingBox));
        }

        static public Vector2 ClampToRectangle(Vector2 point, IRectangle rectangle)
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

        static public IRectangle ClampToRectangle(IRectangle inner, IRectangle outer)
        {
            return new CenteredRectangle(ClampToRectangle(inner.Center, new CenteredRectangle(outer.Center, outer.Size - inner.Size)), inner.Size);
        }
    }
}