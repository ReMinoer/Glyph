using System;

namespace Glyph.Math
{
    public interface IRange<T>
        where T : IComparable<T>
    {
        T Min { get; }
        T Max { get; }
        bool Contains(T value);
        bool Intersects(IRange<T> range);
    }
}