using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Math
{
    public struct Range<T> : IRange<T>
        where T : IComparable<T>
    {
        private T _min;
        private T _max;

        public T Min
        {
            get => _min;
            set
            {
                if (value.CompareTo(_max) > 0)
                    throw new ArgumentOutOfRangeException();

                _min = value;
            }
        }

        public T Max
        {
            get => _max;
            set
            {
                if (value.CompareTo(_min) < 0)
                    throw new ArgumentOutOfRangeException();

                _max = value;
            }
        }

        public Range(T min, T max)
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException();

            _min = min;
            _max = max;
        }

        static public Range<T> FromValues(T first, T second)
        {
            return first.CompareTo(second) <= 0 ? new Range<T>(first, second) : new Range<T>(second, first);
        }

        static public Range<T> FromValues(IEnumerable<T> values)
        {
            return FromValues(values.ToArray());
        }

        static public Range<T> FromValues(params T[] values)
        {
            return new Range<T>(values.Min(), values.Max());
        }

        public void Set(T min, T max)
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException();

            _min = min;
            _max = max;
        }

        public bool Contains(T value)
        {
            return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
        }

        public bool Intersects(Range<T> range) => RangeUtils.Intersects(this, range);
        public bool Intersects(IRange<T> range) => RangeUtils.Intersects(this, range);
    }
}