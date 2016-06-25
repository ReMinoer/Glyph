using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class PointExtension
    {
        static public Vector2 ToVector2(this Point value)
        {
            return new Vector2(value.X, value.Y);
        }

        static public float DistanceTo(this Point value, Point other)
        {
            return (float)System.Math.Sqrt(System.Math.Pow(value.X - other.X, 2) + System.Math.Pow(value.Y - other.Y, 2));
        }
    }
}