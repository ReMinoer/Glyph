using System;

namespace Glyph
{
    public enum EpsilonUnit
    {
        Second,
        Millisecond
    }

    static public class Epsilon
    {
        static private float Default = 1e-10f;
        static private float Second = 1e-5f;
        static private float Millisecond = 1e-5f;

        static public bool EpsilonEquals(this float value, float other) => value.EpsilonEquals(other, Default);
        static public bool EpsilonEquals(this float value, float other, EpsilonUnit unit) => value.EpsilonEquals(other, unit.GetEpsilon());
        static public bool EpsilonEquals(this float value, float other, float epsilon)
        {
            return (value - other).EqualsZero(epsilon);
        }

        static public bool EqualsZero(this float value) => value.EqualsZero(Default);
        static public bool EqualsZero(this float value, float epsilon)
        {
            return System.Math.Abs(value) < epsilon;
        }

        static public float GetEpsilon(this EpsilonUnit unit)
        {
            switch (unit)
            {
                case EpsilonUnit.Second: return Second;
                case EpsilonUnit.Millisecond: return Millisecond;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}