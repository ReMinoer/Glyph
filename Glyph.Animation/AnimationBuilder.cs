using System.Collections;
using System.Collections.Generic;

namespace Glyph.Animation
{
    // TODO : AnimationBuilder basé sur des points
    public class AnimationBuilder<T> : IEnumerable<AnimationStep<T>>
    {
        private readonly List<AnimationStep<T>> _list;
        public bool Loop { get; set; }

        public AnimationBuilder()
        {
            _list = new List<AnimationStep<T>>();
        }

        public AnimationStepDelegate<T> this[float instant]
        {
            set { this[instant, instant] = value; }
        }

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

        public StandardAnimation<T> Generate()
        {
            var animation = new StandardAnimation<T>(_list)
            {
                Loop = Loop
            };

            return animation;
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