using System.Collections;
using System.Collections.Generic;

namespace Glyph.Animation
{
    public class AnimationBuilder<T> : IEnumerable<AnimationStep<T>>
    {
        private readonly List<AnimationStep<T>> _list;

        public AnimationStepDelegate<T> this[float begin, float end]
        {
            set
            {
                var step = new AnimationStep<T>
                {
                    Begin = begin,
                    End = end,
                    Action = value
                };

                _list.Add(step);
            }
        }

        public AnimationBuilder()
        {
            _list = new List<AnimationStep<T>>();
        }

        public StandardAnimation<T> Generate()
        {
            return new StandardAnimation<T>(_list);
        }

        public IEnumerator<AnimationStep<T>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}