using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public delegate bool IntersectionDelegate<in TShape, in TOther>(TShape collider, TOther other)
        where TShape : IArea where TOther : IArea;

    public delegate bool CollisionDelegate<in TShape, in TOther>(TShape collider, TOther other, out Vector2 correction)
        where TShape : IArea where TOther : IArea;

    static public class IntersectionUtils
    {
        static public IEnumerable<Vector2> TriangulationVertices(this ITriangulableShape shape)
        {
            return shape.TriangulationIndices?.Select(shape.GetIndexedVertex) ?? shape.Vertices;
        }

        static public IEnumerable<Triangle> Triangles(this ITriangulableShape shape)
        {
            using (IEnumerator<Vector2> enumerator = shape.TriangulationVertices().GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    yield break;

                var triangle = new Triangle();
                triangle.P0 = enumerator.Current;
                enumerator.MoveNext();
                triangle.P1 = enumerator.Current;
                enumerator.MoveNext();
                triangle.P2 = enumerator.Current;
                yield return triangle;

                if (shape.StripTriangulation)
                {
                    while (enumerator.MoveNext())
                    {
                        triangle = new Triangle
                        {
                            P0 = triangle.P2,
                            P1 = triangle.P1,
                            P2 = enumerator.Current
                        };
                        yield return triangle;
                    }
                }

                while (enumerator.MoveNext())
                {
                    triangle = new Triangle();
                    triangle.P0 = enumerator.Current;
                    enumerator.MoveNext();
                    triangle.P1 = enumerator.Current;
                    enumerator.MoveNext();
                    triangle.P2 = enumerator.Current;
                    yield return triangle;
                }
            }
        }

        static public bool Intersects(this Segment segmentA, Segment segmentB) => Intersects(segmentA, segmentB, out _);

        // https://stackoverflow.com/a/565282/3333753
        static public bool Intersects(this Segment segmentA, Segment segmentB, out Segment? intersection)
        {
            Vector2 vectorA = segmentA.Vector;
            Vector2 vectorB = segmentB.Vector;

            Vector2 p0Diff = segmentB.P0 - segmentA.P0;
            float b = p0Diff.Cross(vectorB);

            float abCross = vectorA.Cross(vectorB);

            // Check if segments are parallel
            if (abCross.EqualsZero())
            {
                // Check if segments are colinear
                if (b.EqualsZero())
                {
                    float aaDot = Vector2.Dot(vectorA, vectorA);
                    float baDot = Vector2.Dot(vectorB, vectorA);
                    float t0 = Vector2.Dot(p0Diff, vectorA) / aaDot;
                    float t1 = t0 + baDot / aaDot;
                    Range<float> t01 = baDot >= 0 ? new Range<float>(t0, t1) : new Range<float>(t1, t0);

                    // Check if segments are overlapping
                    if (RangeUtils.Intersects(t01, new Range<float>(0, 1)))
                    {
                        intersection = new Segment(segmentA.P0 + t01.Min * vectorA, segmentA.P0 + t01.Max * vectorA);
                        return true;
                    }
                }
            }
            else
            {
                float a = p0Diff.Cross(vectorA);

                float t = a / abCross;
                float u = b / abCross;

                // Check if intersect at one point
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    Vector2 intersectionPoint = segmentA.P0 + t * vectorA;
                    intersection = new Segment(intersectionPoint, intersectionPoint);
                    return true;
                }
            }

            intersection = null;
            return false;
        }

        // https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm
        static public bool Intersects(this Segment segment, Circle circle)
        {
            Vector2 d = segment.Vector;
            Vector2 f = segment.P0 - circle.Center;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - circle.Radius * circle.Radius;

            float discriminant = b * b - 4 * a * c;
            if (discriminant >= 0)
            {
                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.

                discriminant = (float)System.Math.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                // Cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit),
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                // Impale, Poke
                if (t1 >= 0 && t1 <= 1)
                    return true;

                // ExitWound
                if (t2 >= 0 && t2 <= 1)
                    return true;

                // CompletelyInside
                if (t1 < 0 && t2 > 1)
                    return true;
            }

            return false;
        }

        static public bool Intersects(this Circle circle, Segment segment) => Intersects(segment, circle);
        static public bool Intersects(this Circle circleA, Circle circleB) => Collides(circleA, circleB, out _);

        static public bool Intersects<T>(this T edgedShape, Segment segment)
            where T : IEdgedShape
        {
            return segment.Vertices.Any(edgedShape.ContainsPoint)
                   || edgedShape.Edges.Any(e => Intersects(segment, e));
        }

        static public bool Intersects<T>(this T edgedShape, Circle circle)
            where T : IEdgedShape
        {
            return edgedShape.Vertices.Any(circle.ContainsPoint)
                   || edgedShape.ContainsPoint(circle.Center)
                   || edgedShape.Edges.Any(e => Intersects(e, circle));
        }

        static public bool Intersects<T>(this Segment segment, T edgedShape) where T : IEdgedShape => Intersects(edgedShape, segment);
        static public bool Intersects<T>(this Circle circle, T edgedShape) where T : IEdgedShape => Intersects(edgedShape, circle);

        static public bool Intersects<TA, TB>(this TA edgedShapeA, TB edgedShapeB)
            where TA : IEdgedShape
            where TB : IEdgedShape
        {
            return edgedShapeA.Vertices.Any(edgedShapeB.ContainsPoint)
                   || edgedShapeB.Vertices.Any(edgedShapeA.ContainsPoint)
                   || edgedShapeA.Edges.Any(eA => edgedShapeB.Edges.Any(eB => Intersects(eA, eB)));
        }

        static public bool Intersects<T>(this RangeArea rangeArea, T edgedShape)
            where T : IEdgedShape
        {
            return Range<float>.FromValues(edgedShape.Vertices.Select(x => x.Coordinate(rangeArea.Axis))).Intersects(rangeArea);
        }

        static public bool Intersects(this RangeArea rangeArea, Circle circle)
        {
            float centerCoordinate = circle.Center.Coordinate(rangeArea.Axis);
            return new Range<float>(centerCoordinate - circle.Radius, centerCoordinate + circle.Radius).Intersects(rangeArea);
        }

        static public bool Intersects<T>(this T edgedShape, RangeArea rangeArea) where T : IEdgedShape => Intersects(rangeArea, edgedShape);
        static public bool Intersects(this Circle circle, RangeArea rangeArea) => Intersects(rangeArea, circle);

        static public bool Intersects(this TopLeftRectangle rectangleA, TopLeftRectangle rectangleB)
        {
            return Intersects(rectangleA, rectangleB, out TopLeftRectangle _);
        }

        static public bool Intersects(this TopLeftRectangle rectangleA, TopLeftRectangle rectangleB, out TopLeftRectangle intersection)
        {
            float left = MathHelper.Max(rectangleA.Left, rectangleB.Left);
            float right = MathHelper.Min(rectangleA.Right, rectangleB.Right);
            float top = MathHelper.Max(rectangleA.Top, rectangleB.Top);
            float bottom = MathHelper.Min(rectangleA.Bottom, rectangleB.Bottom);

            if (left >= right || top >= bottom)
            {
                intersection = TopLeftRectangle.Void;
                return false;
            }

            intersection = new TopLeftRectangle(left, top, right - left, bottom - top);
            return true;
        }

        static public bool Intersects(this TopLeftRectangle rectangle, Circle circle) => Collides(rectangle, circle, out _);
        static public bool Intersects(this Circle circle, TopLeftRectangle rectangle) => Collides(rectangle, circle, out _);

        static public bool Collides(TopLeftRectangle rectangle, TopLeftRectangle other, out Vector2 correction)
        {
            if (Intersects(rectangle, other, out TopLeftRectangle intersection))
            {
                bool isWiderThanTall = intersection.Width > intersection.Height;

                if (isWiderThanTall)
                {
                    correction = rectangle.Center.Y <= other.Center.Y ? new Vector2(0, -intersection.Height) : new Vector2(0, intersection.Height);
                }
                else
                {
                    correction = rectangle.Center.X <= other.Center.X ? new Vector2(-intersection.Width, 0) : new Vector2(intersection.Width, 0);
                }

                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool Collides(Circle circleA, Circle circleB, out Vector2 correction)
        {
            Vector2 centersDistance = circleA.Center - circleB.Center;

            float radiusIntersection = circleA.Radius + circleB.Radius - centersDistance.Length();
            if (radiusIntersection > 0)
            {
                correction = radiusIntersection * centersDistance.Normalized();
                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool Collides(TopLeftRectangle rectangle, Circle circle, out Vector2 correction)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = MathHelper.Clamp(circle.Center.X, rectangle.Left, rectangle.Right);
            float closestY = MathHelper.Clamp(circle.Center.Y, rectangle.Top, rectangle.Bottom);
            var closest = new Vector2(closestX, closestY);

            // Calculate the distance between the circle's center and this closest point
            Vector2 distance = circle.Center - closest;

            // If the distance is less than the circle's radius, an intersection occurs
            float radiusIntersection = circle.Radius - distance.Length();

            if (radiusIntersection > 0)
            {
                correction = radiusIntersection * (closest - rectangle.Center).Normalized();
                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool Collides(Circle circle, TopLeftRectangle rectangle, out Vector2 correction)
        {
            bool result = Collides(rectangle, circle, out correction);
            correction = correction.Inverse();
            return result;
        }
    }
}