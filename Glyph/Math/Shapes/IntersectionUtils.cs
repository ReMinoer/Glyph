using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public delegate bool IntersectionDelegate<in TShape, in TOther>(TShape collider, TOther other)
        where TShape : IShape
        where TOther : IShape;
    public delegate bool CollisionDelegate<in TShape, in TOther>(TShape collider, TOther other, out Vector2 correction)
        where TShape : IShape
        where TOther : IShape;

    static public class IntersectionUtils
    {
        static public bool RectangleWithRectangle(TopLeftRectangle rectangleA, TopLeftRectangle rectangleB)
        {
            TopLeftRectangle intersection;
            return RectangleWithRectangle(rectangleA, rectangleB, out intersection);
        }

        static public bool RectangleWithRectangle(TopLeftRectangle rectangleA, TopLeftRectangle rectangleB, out TopLeftRectangle intersection)
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
        
        static public bool RectangleWithRectangle(TopLeftRectangle rectangle, TopLeftRectangle other, out Vector2 correction)
        {
            TopLeftRectangle intersection;
            if (RectangleWithRectangle(rectangle, other, out intersection))
            {
                bool isWiderThanTall = intersection.Width > intersection.Height;

                if (isWiderThanTall)
                {
                    correction = rectangle.Center.Y <= other.Center.Y
                        ? new Vector2(0, -intersection.Height)
                        : new Vector2(0, intersection.Height);
                }
                else
                {
                    correction = rectangle.Center.X <= other.Center.X
                        ? new Vector2(-intersection.Width, 0)
                        : new Vector2(intersection.Width, 0);
                }

                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool CircleWithCircle(Circle circleA, Circle circleB)
        {
            Vector2 correction;
            return CircleWithCircle(circleA, circleB, out correction);
        }

        static public bool CircleWithCircle(Circle circleA, Circle circleB, out Vector2 correction)
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

        static public bool RectangleWithCircle(TopLeftRectangle rectangle, Circle circle)
        {
            Vector2 correction;
            return RectangleWithCircle(rectangle, circle, out correction);
        }

        static public bool RectangleWithCircle(TopLeftRectangle rectangle, Circle circle, out Vector2 correction)
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

        static public bool RectangleWithCircle(Circle circle, TopLeftRectangle rectangle)
        {
            return RectangleWithCircle(rectangle, circle);
        }

        static public bool CircleWithRectangle(Circle circle, TopLeftRectangle rectangle, out Vector2 correction)
        {
            bool result = RectangleWithCircle(rectangle, circle, out correction);
            correction = correction.Inverse();
            return result;
        }
    }
}