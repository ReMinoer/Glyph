using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    static public class IntersectionUtils
    {
        static public bool TwoRectangles(IRectangle rectangleA, IRectangle rectangleB)
        {
            IRectangle intersection;
            return TwoRectangles(rectangleA, rectangleB, out intersection);
        }

        static public bool TwoRectangles(IRectangle rectangleA, IRectangle rectangleB, out IRectangle intersection)
        {
            float left = MathHelper.Max(rectangleA.Left, rectangleB.Left);
            float right = MathHelper.Min(rectangleA.Right, rectangleB.Right);
            float top = MathHelper.Max(rectangleA.Top, rectangleB.Top);
            float bottom = MathHelper.Min(rectangleA.Bottom, rectangleB.Bottom);

            if (left > right || top > bottom)
            {
                intersection = new CenteredRectangle(Vector2.Zero, 0, 0);
                return false;
            }

            Vector2 center = new Vector2(left + right, bottom + top) / 2;
            intersection = new CenteredRectangle(center, right - left, bottom - top);
            return true;
        }

        static public bool TwoCircles(ICircle circleA, ICircle circleB)
        {
            float radiusIntersection;
            return TwoCircles(circleA, circleB, out radiusIntersection);
        }

        static public bool TwoCircles(ICircle circleA, ICircle circleB, out float radiusIntersection)
        {
            radiusIntersection = circleA.Radius + circleB.Radius - (circleA.Center - circleB.Center).Length();
            return radiusIntersection >= 0;
        }

        static public bool RectangleAndCircle(IRectangle rectangle, ICircle circle)
        {
            Vector2 correction;
            return RectangleAndCircle(rectangle, circle, out correction);
        }

        static public bool RectangleAndCircle(IRectangle rectangle, ICircle circle, out Vector2 correction)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = MathHelper.Clamp(circle.Center.X, rectangle.Left, rectangle.Right);
            float closestY = MathHelper.Clamp(circle.Center.Y, rectangle.Top, rectangle.Bottom);
            var closest = new Vector2(closestX, closestY);

            // Calculate the distance between the circle's center and this closest point
            Vector2 distance = circle.Center - closest;

            // If the distance is less than the circle's radius, an intersection occurs
            float radiusIntersection = circle.Radius - distance.Length();
            correction = radiusIntersection * (closest - rectangle.Center);
            return radiusIntersection >= 0;
        }
    }
}