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
            return (circleA.Center - circleB.Center).Length() <= circleA.Radius + circleB.Radius;
        }

        static public bool RectangleAndCircle(IRectangle rectangle, ICircle circle)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = MathHelper.Clamp(circle.Center.X, rectangle.Left, rectangle.Right);
            float closestY = MathHelper.Clamp(circle.Center.Y, rectangle.Top, rectangle.Bottom);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = circle.Center.X - closestX;
            float distanceY = circle.Center.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared <= (circle.Radius * circle.Radius);
        }
    }
}