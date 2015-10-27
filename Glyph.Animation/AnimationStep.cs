using System;

namespace Glyph.Animation
{
    public delegate void AnimationStepDelegate<in T>(T animatable, float advance);

    public class AnimationStep<T> : IComparable<AnimationStep<T>>
    {
        public float Begin { get; set; }
        public float End { get; set; }
        public AnimationStepDelegate<T> Action { get; set; }

        public void Apply(T animatable, float advance)
        {
            Action(animatable, advance);
        }

        public int CompareTo(AnimationStep<T> other)
        {
            return Begin.CompareTo(other.Begin);
        }
    }
}