using System;

namespace Glyph.Animation
{
    public struct AnimationKeyPoint<T> : IComparable<AnimationKeyPoint<T>>
    {
        public float Time { get; set; }
        public T Value { get; set; }
        public ValueEasingDelegate<T> Easing { get; set; }

        public int CompareTo(AnimationKeyPoint<T> other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}