using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class Angle
    {
        static public float ToRadians(float degrees)
        {
            return degrees / 180 * MathHelper.Pi;
        }

        static public float ToDegrees(float radians)
        {
            return radians * 180 / MathHelper.Pi;
        }

        static public float ToPositiveRadians(float degrees)
        {
            float result = ToRadians(degrees);
            result %= MathHelper.TwoPi;
            result += MathHelper.TwoPi;
            result %= MathHelper.TwoPi;

            return result;
        }

        static public float ToPositiveDegrees(float radians)
        {
            float result = radians;
            result %= MathHelper.TwoPi;
            result += MathHelper.TwoPi;
            result %= MathHelper.TwoPi;
            result = ToDegrees(result);

            return result;
        }

        static public float ToSignedRadians(float degrees)
        {
            (float sin, float cos) = MathF.SinCos(ToRadians(degrees));
            return MathF.Atan2(sin,cos);
        }

        static public float ToSignedDegrees(float radians)
        {
            return ToDegrees(MathF.Atan2(MathF.Sin(radians), MathF.Cos(radians)));
        }
    }
}