using System;

namespace Glyph
{
    static public class FloatExtension
    {
        static public bool EpsilonEquals(this float value, float other)
        {
            return System.Math.Abs(value - other) < float.Epsilon;
        }

        static public bool EqualsZero(this float value)
        {
            return System.Math.Abs(value) < float.Epsilon;
        }
    }
}