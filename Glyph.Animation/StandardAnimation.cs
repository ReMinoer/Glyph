using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Animation
{
    public class StandardAnimation<T> : IAnimation, IEnumerable<AnimationStep<T>>
    {
        private readonly List<AnimationStep<T>> _chronology;
        public float Duration { get; private set; }

        public StandardAnimation(IEnumerable<AnimationStep<T>> animationSteps)
        {
            _chronology = new List<AnimationStep<T>>();
            _chronology.AddRange(animationSteps);
            _chronology.Sort();

            Duration = this.Max(x => x.End);
        }

        public IEnumerator<AnimationStep<T>> GetEnumerator()
        {
            return _chronology.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}