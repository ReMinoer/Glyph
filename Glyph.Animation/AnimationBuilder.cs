using System.Collections;
using System.Collections.Generic;

namespace Glyph.Animation
{
    public class AnimationBuilder<T> : IAnimationBuilder<T>, IEnumerable<AnimationTransition<T>>
    {
        private readonly List<AnimationTransition<T>> _list;
        public bool Loop { get; set; }

        public AnimationBuilder()
        {
            _list = new List<AnimationTransition<T>>();
        }

        public AnimationTransitionDelegate<T> this[float instant]
        {
            set { this[instant, instant] = value; }
        }

        public AnimationTransitionDelegate<T> this[float begin, float end]
        {
            set
            {
                var step = new AnimationTransition<T>
                {
                    Begin = begin,
                    End = end,
                    Action = value
                };

                _list.Add(step);
            }
        }

        public IAnimation<T> Create()
        {
            var animation = new Animation<T>(_list)
            {
                Loop = Loop
            };

            return animation;
        }

        public IEnumerator<AnimationTransition<T>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}