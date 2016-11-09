using System;

namespace Glyph.Animation
{
    public delegate void AnimationTransitionDelegate<T>(ref T animatable, float advance);

    public struct AnimationTransition<T> : IComparable<AnimationTransition<T>>
    {
        public float Begin { get; set; }
        public float End { get; set; }
        public AnimationTransitionDelegate<T> Action { get; set; }

        public void Apply(ref T animatable, float advance)
        {
            Action?.Invoke(ref animatable, advance);
        }

        public int CompareTo(AnimationTransition<T> other)
        {
            return Begin.CompareTo(other.Begin);
        }
    }
}