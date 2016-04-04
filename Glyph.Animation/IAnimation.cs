using System.Collections.Generic;

namespace Glyph.Animation
{
    public interface IAnimation
    {
        float Duration { get; }
        bool Loop { get; }
    }

    public interface IAnimation<T> : IAnimation, IEnumerable<AnimationTransition<T>>
    {
    }
}