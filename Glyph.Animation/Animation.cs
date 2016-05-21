using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Animation
{
    public class Animation<T> : IAnimation<T>
    {
        private readonly List<AnimationTransition<T>> _chronology;
        public float Duration { get; private set; }
        public bool Loop { get; set; }

        public Animation(IEnumerable<AnimationTransition<T>> animationSteps)
        {
            _chronology = new List<AnimationTransition<T>>();
            _chronology.AddRange(animationSteps);
            _chronology.Sort();

            Duration = this.Any() ? this.Max(x => x.End) : 0;
        }

        public IEnumerator<AnimationTransition<T>> GetEnumerator()
        {
            return _chronology.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}