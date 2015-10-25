using System;

namespace Glyph
{
    static public class FloatExtension
    {
        static public bool EqualsZero(this float value)
        {
            return Math.Abs(value) < float.Epsilon;
        }
    }
}