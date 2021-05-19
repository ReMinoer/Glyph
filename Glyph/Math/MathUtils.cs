using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    static public class MathUtils
    {
        static public Vector2 GetCenter(params Vector2[] values)
        {
            return GetCenter(values.AsEnumerable());
        }

        static public Vector2 GetCenter(IEnumerable<Vector2> values)
        {
            if (values == null)
                throw new ArgumentNullException();

            Vector2 center = Vector2.Zero;
            int i = 0;
            foreach (Vector2 value in values)
            {
                center += value;
                i++;
            }

            return center / i;
        }

        static public TopLeftRectangle GetBoundingBox(params Vector2[] points)
        {
            return GetBoundingBox(points.AsEnumerable());
        }

        static public TopLeftRectangle GetBoundingBox(IEnumerable<Vector2> points)
        {
            if (points == null)
                return TopLeftRectangle.Void;

            using (IEnumerator<Vector2> enumerator = points.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return TopLeftRectangle.Void;

                Vector2 point = enumerator.Current;

                float left = point.X;
                float right = point.X;
                float top = point.Y;
                float bottom = point.Y;

                while (enumerator.MoveNext())
                {
                    point = enumerator.Current;

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
        }

        static public TopLeftRectangle GetBoundingBox(params TopLeftRectangle[] rectangles)
        {
            return GetBoundingBox(rectangles.AsEnumerable());
        }

        static public TopLeftRectangle GetBoundingBox(IEnumerable<TopLeftRectangle> rectangles)
        {
            if (rectangles == null)
                return TopLeftRectangle.Void;

            using (IEnumerator<TopLeftRectangle> enumerator = rectangles.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return TopLeftRectangle.Void;

                TopLeftRectangle rectangle = enumerator.Current;

                float left = rectangle.Left;
                float right = rectangle.Right;
                float top = rectangle.Top;
                float bottom = rectangle.Bottom;

                while (enumerator.MoveNext())
                {
                    rectangle = enumerator.Current;

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
        }

        static public TopLeftRectangle GetBoundingBox(params IArea[] areas)
        {
            return GetBoundingBox(areas.AsEnumerable());
        }

        static public TopLeftRectangle GetBoundingBox(IEnumerable<IArea> areas)
        {
            return GetBoundingBox(areas.Select(x => x.BoundingBox));
        }

        static public Rectangle GetBoundingBox(params Point[] points)
        {
            return GetBoundingBox(points.AsEnumerable());
        }

        static public Rectangle GetBoundingBox(IEnumerable<Point> points)
        {
            if (points == null)
                return Rectangle.Empty;

            using (IEnumerator<Point> enumerator = points.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return Rectangle.Empty;

                Point point = enumerator.Current;

                int left = point.X;
                int right = point.X;
                int top = point.Y;
                int bottom = point.Y;

                while (enumerator.MoveNext())
                {
                    point = enumerator.Current;

                    if (point.X < left)
                        left = point.X;
                    if (point.X > right)
                        right = point.X;
                    if (point.Y < top)
                        top = point.Y;
                    if (point.Y > bottom)
                        bottom = point.Y;
                }

                return new Rectangle(left, top, right - left, bottom - top);
            }
        }

        // https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd
        static public Vector2 GetClosestToPointOnLine(Vector2 point, Segment openSegment)
        {
            Vector2 p0 = openSegment.P0;
            Vector2 v = openSegment.Vector;
            float lambda = Vector2.Dot(point - p0, v) / Vector2.Dot(v, v);

            return p0 + v * lambda;
        }

        static public Vector2 GetClosestToPointOnSegment(Vector2 point, Segment closedSegment)
        {
            Vector2 p0 = closedSegment.P0;
            Vector2 v = closedSegment.Vector;
            float lambda = Vector2.Dot(point - p0, v) / Vector2.Dot(v, v);

            if (lambda <= 0)
                return p0;
            if (lambda >= 1)
                return closedSegment.P1;

            return p0 + v * lambda;
        }
    }
}