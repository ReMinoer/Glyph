using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;

namespace Glyph.Animation
{
    public class PropertiesAnimationBuilder<T> : IAnimationBuilder<T>, IEnumerable
    {
        private readonly List<AnimationTransition<T>> _transitions;
        private readonly List<LoopedAnimationTransition> _loopedTransitions;
        private float _linearDuration;
        public bool Loop { get; set; }

        public PropertiesAnimationBuilder()
        {
            _transitions = new List<AnimationTransition<T>>();
            _loopedTransitions = new List<LoopedAnimationTransition>();
        }

        public void Add<TProperty>(Action<T, TProperty> setter, IAnimation<TProperty> animation)
        {
            var transitions = new List<AnimationTransition<T>>();
            foreach (AnimationTransition<TProperty> propertyTransition in animation)
            {
                var transition = new AnimationTransition<T>
                {
                    Begin = propertyTransition.Begin,
                    End = propertyTransition.End,
                    Action = (ref T animatable, float advance) =>
                    {
                        TProperty property = default(TProperty);
                        propertyTransition.Action(ref property, advance);
                        setter(animatable, property);
                    }
                };

                transitions.Add(transition);
            }

            if (animation.Loop)
                _loopedTransitions.Add(new LoopedAnimationTransition(transitions));
            else
            {
                _transitions.AddRange(transitions);

                if (animation.Duration > _linearDuration)
                    _linearDuration = animation.Duration;
            }
        }
        
        public void Add<TProperty>(Action<T, TProperty> setter, IAnimationBuilder<TProperty> animationBuilder)
        {
            Add(setter, animationBuilder.Create());
        }

        public IAnimation<T> Create()
        {
            float loopDuration = GetLeastCommonMultiple(_loopedTransitions.Select(x => x.Duration).ToArray());
            float linearDuration = _transitions.Any() ? _transitions.Max(x => x.End) : 0;

            float duration = loopDuration;
            while (duration < linearDuration)
                duration += loopDuration;

            foreach (LoopedAnimationTransition loopedAnimationTransition in _loopedTransitions)
            {
                for (float i = 0; i < duration; i += loopedAnimationTransition.Duration)
                {
                    foreach (AnimationTransition<T> animationTransition in loopedAnimationTransition.Transitions)
                    {
                        var transition = new AnimationTransition<T>
                        {
                            Begin = animationTransition.Begin + i,
                            End = animationTransition.Begin + i,
                            Action = animationTransition.Action
                        };
                        _transitions.Add(transition);
                    }
                }
            }

            var animation = new Animation<T>(_transitions)
            {
                Loop = Loop
            };

            return animation;
        }

        private float GetLeastCommonMultiple(params float[] values)
        {
            float leastCommonMultiple = values[0];
            for (int i = 1; i < values.Length; i++)
                leastCommonMultiple = GetLeastCommonMultiple(leastCommonMultiple, values[i]);

            return leastCommonMultiple;
        }

        private float GetLeastCommonMultiple(float a, float b)
        {
            float x = a;
            float y = b;

            while (!MathUtils.FloatEquals(x,y))
            {
                if (a < b)
                    x += a;
                else
                    y += b;
            }

            return x;
        }

        private sealed class LoopedAnimationTransition
        {
            public IList<AnimationTransition<T>> Transitions { get; private set; }
            public float Duration { get; private set; }

            public LoopedAnimationTransition(IList<AnimationTransition<T>> transitions)
            {
                Transitions = transitions;
                Duration = transitions.Max(x => x.End);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _transitions.GetEnumerator();
        }
    }
}