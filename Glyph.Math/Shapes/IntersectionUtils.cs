using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public static class IntersectionUtils
    {
        public static bool TwoRectangles(IRectangle rectangleA, IRectangle rectangleB)
        {
            IRectangle intersection;
            return TwoRectangles(rectangleA, rectangleB, out intersection);
        }

        public static bool TwoRectangles(IRectangle rectangleA, IRectangle rectangleB, out IRectangle intersection)
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

        public static bool TwoCircles(ICircle circleA, ICircle circleB)
        {
            return (circleA.Center - circleB.Center).Length() <= circleA.Radius + circleB.Radius;
        }

        public static bool RectangleAndCircle(IRectangle rectangle, ICircle circle)
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