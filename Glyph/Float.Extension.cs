using System;

namespace Glyph
{
    static public class FloatExtension
    {
        static public bool EqualsZero(this float value)
        {
            return System.Math.Abs(value) < float.Epsilon;
        }
    }
}