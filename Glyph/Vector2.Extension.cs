using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class Vector2Extension
    {
        static public bool EpsilonEquals(this Vector2 value, Vector2 other)
        {
            return (value - other).EqualsZero();
        }

        static public bool EqualsZero(this Vector2 value)
        {
            return value.X.EqualsZero() && value.Y.EqualsZero();
        }

        static public Vector2 Normalized(this Vector2 value)
        {
            return value == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(value);
        }

        static public bool IsNormalizedCoordinates(this Vector2 point)
        {
            return point.X >= 0 && point.X <= 1 && point.Y >= 0 && point.Y <= 1;
        }

        static public Vector2 Abs(this Vector2 value)
        {
            return new Vector2(System.Math.Abs(value.X), System.Math.Abs(value.Y));
        }

        static public float Coordinate(this Vector2 value, Axis axis)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return value.X;
                case Axis.Vertical:
                    return value.Y;
                default:
                    throw new NotSupportedException();
            }
        }

        static public float Cross(this Vector2 value, Vector2 other)
        {
            return value.X * other.Y - value.Y * other.X;
        }

        static public Vector2 ClampLength(this Vector2 value, float lengthMax)
        {
            return value.Length() > lengthMax ? value.Normalized() * lengthMax : value;
        }

        static public Vector3 ToVector3(this Vector2 value)
        {
            return new Vector3(value.X, value.Y, 0);
        }

        static public float? ToRotation(this Vector2 value)
        {
            if (value == Vector2.Zero)
                return null;

            value = Vector2.Normalize(value);
            return (float)System.Math.Atan2(value.Y, value.X);
        }

        static public Vector2 ToRotatedVector(this float value)
        {
            return new Vector2((float)System.Math.Cos(value), (float)System.Math.Sin(value));
        }

        static public Vector2 SetX(this Vector2 value, float x)
        {
            return new Vector2(x, value.Y);
        }

        static public Vector2 SetY(this Vector2 value, float y)
        {
            return new Vector2(value.X, y);
        }

        static public Vector2 VectorX(this Vector2 value)
        {
            return new Vector2(value.X, 0);
        }

        static public Vector2 VectorY(this Vector2 value)
        {
            return new Vector2(0, value.Y);
        }

        static public Vector2 Add(this Vector2 value, float v)
        {
            return new Vector2(value.X + v, value.Y + v);
        }

        static public Vector2 Add(this Vector2 value, float x, float y)
        {
            return new Vector2(value.X + x, value.Y + y);
        }

        static public Vector2 Substract(this Vector2 value, float v)
        {
            return new Vector2(value.X - v, value.Y - v);
        }

        static public Vector2 Substract(this Vector2 value, float x, float y)
        {
            return new Vector2(value.X - x, value.Y - y);
        }

        static public Vector2 Multiply(this Vector2 value, float x, float y)
        {
            return new Vector2(value.X * x, value.Y * y);
        }

        static public Vector2 Divide(this Vector2 value, float x, float y)
        {
            return new Vector2(value.X / x, value.Y / y);
        }

        static public Vector2 Orthogonal(this Vector2 value)
        {
            return new Vector2(-value.Y, value.X);
        }

        static public Vector2 Inverse(this Vector2 value)
        {
            return new Vector2(-value.X, -value.Y);
        }

        static public Vector2 Rotate(this Vector2 value, float angle)
        {
            float? currentAngle = value.ToRotation();
            if (currentAngle == null)
                return value;

            float newAngle = MathHelper.WrapAngle(currentAngle.Value + angle);
            return newAngle.ToRotatedVector() * value.Length();
        }

        static public Vector2 RotateToward(this Vector2 value, Vector2 directionGoal, float angularSpeed, float deltaTime)
        {
            float? angle = value.ToRotation();
            if (angle == null)
                return value;

            float? angleGoal = directionGoal.ToRotation();
            if (angleGoal == null)
                return value;

            float diff = angleGoal.Value - angle.Value;
            if (diff.EqualsZero())
                return value;

            float speedSign = System.Math.Sign(diff);
            if (diff > MathHelper.Pi || diff < -MathHelper.Pi)
                speedSign *= -1;

            float newAngle = angle.Value + speedSign * angularSpeed * deltaTime;
            float newDiff = angleGoal.Value - newAngle;
            if (diff * newDiff < 0)
                newAngle = angleGoal.Value;

            return new Vector2((float)System.Math.Cos(newAngle), (float)System.Math.Sin(newAngle));
        }

        static public Point Discretize(this Vector2 value, Vector2 localPoint)
        {
            var result = new Point((int)(localPoint.X / value.X), (int)(localPoint.Y / value.Y));

            if (localPoint.X < 0)
                result -= new Point(1, 0);
            if (localPoint.Y < 0)
                result -= new Point(0, 1);

            return result;
        }

        static public Vector2 Integrate(this Vector2 value, Point gridPoint)
        {
            return new Vector2(gridPoint.X * value.X, gridPoint.Y * value.Y);
        }

        static public float Transition(this float value, float goal, float speed, GameTime gameTime)
        {
            return Transition(value, goal, speed, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        static public Vector2 Transition(this Vector2 value, Vector2 goal, float speed, GameTime gameTime)
        {
            return Transition(value, goal, speed, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        static public float Transition(this float value, float goal, float speed, float deltaTime)
        {
            float diff = goal - value;
            if (diff.EqualsZero())
                return value;

            float delta = System.Math.Sign(diff) * speed * deltaTime;
            if (System.Math.Abs(delta) >= System.Math.Abs(diff))
                return goal;

            return value + delta;
        }

        static public Vector2 Transition(this Vector2 value, Vector2 goal, float speed, float deltaTime)
        {
            Vector2 diff = goal - value;
            if (diff.EqualsZero())
                return value;

            Vector2 modif = Vector2.Normalize(diff) * speed * deltaTime;
            if (modif.Length() >= diff.Length())
                return goal;

            return value + modif;
        }
    }
}