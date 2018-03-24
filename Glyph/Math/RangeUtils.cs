using System;

namespace Glyph.Math
{
    static public class RangeUtils
    {
        // http://world.std.com/~swmcd/steven/tech/interval.html
        static public bool Intersects<T>(IRange<T> rangeA, IRange<T> rangeB)
            where T : IComparable<T>
        {
            return rangeB.Min.CompareTo(rangeA.Max) < 0 && rangeA.Min.CompareTo(rangeB.Max) < 0;
        }

        static public bool Intersects<T>(Range<T> rangeA, Range<T> rangeB)
            where T : IComparable<T>
        {
            return rangeB.Min.CompareTo(rangeA.Max) < 0 && rangeA.Min.CompareTo(rangeB.Max) < 0;
        }

        static public bool Intersects<T>(IRange<T> rangeA, IRange<T> rangeB, out Range<T>? range)
            where T : IComparable<T>
        {
            if (Intersects(rangeA, rangeB))
            {
                range = new Range<T> { Min = rangeA.Min.CompareTo(rangeB.Min) >= 0 ? rangeA.Min : rangeB.Min, Max = rangeA.Max.CompareTo(rangeB.Min) <= 0 ? rangeA.Max : rangeB.Max };
                return true;
            }

            range = null;
            return false;
        }

        static public bool Intersects<T>(Range<T> rangeA, Range<T> rangeB, out Range<T>? range)
            where T : IComparable<T>
        {
            if (Intersects(rangeA, rangeB))
            {
                range = new Range<T> { Min = rangeA.Min.CompareTo(rangeB.Min) >= 0 ? rangeA.Min : rangeB.Min, Max = rangeA.Max.CompareTo(rangeB.Min) <= 0 ? rangeA.Max : rangeB.Max };
                return true;
            }

            range = null;
            return false;
        }
    }
}