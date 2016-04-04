using Microsoft.Xna.Framework;
using Glyph;

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
        static public bool RectangleWithRectangle(IRectangle rectangleA, IRectangle rectangleB)
        {
            IRectangle intersection;
            return RectangleWithRectangle(rectangleA, rectangleB, out intersection);
        }

        static public bool RectangleWithRectangle(IRectangle rectangleA, IRectangle rectangleB, out IRectangle intersection)
        {
            float left = Microsoft.Xna.Framework.MathHelper.Max(rectangleA.Left, rectangleB.Left);
            float right = Microsoft.Xna.Framework.MathHelper.Min(rectangleA.Right, rectangleB.Right);
            float top = Microsoft.Xna.Framework.MathHelper.Max(rectangleA.Top, rectangleB.Top);
            float bottom = Microsoft.Xna.Framework.MathHelper.Min(rectangleA.Bottom, rectangleB.Bottom);

            if (left > right || top > bottom)
            {
                intersection = new CenteredRectangle(Vector2.Zero, 0, 0);
                return false;
            }

            Vector2 center = new Vector2(left + right, bottom + top) / 2;
            intersection = new CenteredRectangle(center, right - left, bottom - top);
            return true;
        }
        
        static public bool RectangleWithRectangle(IRectangle rectangle, IRectangle other, out Vector2 correction)
        {
            IRectangle intersection;
            if (RectangleWithRectangle(rectangle, other, out intersection))
            {
                bool isWiderThanTall = intersection.Width > intersection.Height;

                if (isWiderThanTall)
                {
                    correction = rectangle.Top <= other.Top
                        ? new Vector2(0, -intersection.Height)
                        : new Vector2(0, intersection.Height);
                }
                else
                {
                    correction = rectangle.Right >= other.Right
                        ? new Vector2(intersection.Width, 0)
                        : new Vector2(-intersection.Width, 0);
                }

                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool CircleWithCircle(ICircle circleA, ICircle circleB)
        {
            Vector2 correction;
            return CircleWithCircle(circleA, circleB, out correction);
        }

        static public bool CircleWithCircle(ICircle circleA, ICircle circleB, out Vector2 correction)
        {
            Vector2 centersDistance = circleA.Center - circleB.Center;

            float radiusIntersection = circleA.Radius + circleB.Radius - centersDistance.Length();
            if (radiusIntersection >= 0)
            {
                correction = radiusIntersection * centersDistance.Normalized();
                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool RectangleWithCircle(IRectangle rectangle, ICircle circle)
        {
            Vector2 correction;
            return RectangleWithCircle(rectangle, circle, out correction);
        }

        static public bool RectangleWithCircle(IRectangle rectangle, ICircle circle, out Vector2 correction)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = Microsoft.Xna.Framework.MathHelper.Clamp(circle.Center.X, rectangle.Left, rectangle.Right);
            float closestY = Microsoft.Xna.Framework.MathHelper.Clamp(circle.Center.Y, rectangle.Top, rectangle.Bottom);
            var closest = new Vector2(closestX, closestY);

            // Calculate the distance between the circle's center and this closest point
            Vector2 distance = circle.Center - closest;

            // If the distance is less than the circle's radius, an intersection occurs
            float radiusIntersection = circle.Radius - distance.Length();

            if (radiusIntersection >= 0)
            {
                correction = radiusIntersection * (closest - rectangle.Center).Normalized();
                return true;
            }

            correction = Vector2.Zero;
            return false;
        }

        static public bool RectangleWithCircle(ICircle circle, IRectangle rectangle)
        {
            return RectangleWithCircle(rectangle, circle);
        }

        static public bool CircleWithRectangle(ICircle circle, IRectangle rectangle, out Vector2 correction)
        {
            bool result = RectangleWithCircle(rectangle, circle, out correction);
            correction = correction.Inverse();
            return result;
        }
    }
}