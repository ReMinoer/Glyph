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
            IEnumerable<Vector2> enumerable = points as Vector2[] ?? points.ToArray();

            float left = enumerable.Min(x => x.X);
            float right = enumerable.Max(x => x.X);
            float top = enumerable.Min(x => x.Y);
            float bottom = enumerable.Max(x => x.Y);

            return new OriginRectangle(left, top, right - left, bottom - top);
        }

        static public IRectangle GetBoundingBox(params IRectangle[] rectangles)
        {
            return GetBoundingBox(rectangles.AsEnumerable());
        }

        static public IRectangle GetBoundingBox(IEnumerable<IRectangle> rectangles)
        {
            IEnumerable<IRectangle> enumerable = rectangles as IRectangle[] ?? rectangles.ToArray();
            if (!enumerable.Any())
                return null;

            float left = enumerable.Min(x => x.Left);
            float right = enumerable.Max(x => x.Right);
            float top = enumerable.Min(x => x.Top);
            float bottom = enumerable.Max(x => x.Bottom);

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