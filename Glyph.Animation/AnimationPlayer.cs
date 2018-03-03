using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;

namespace Glyph.Animation
{
    public class AnimationPlayer<T> : GlyphComponent, IAnimationPlayer
        where T : class
    {
        private readonly List<AnimationTransition<T>> _stepsToRead;
        private IAnimation<T> _animation;
        public bool UseUnscaledTime { get; set; }
        public WeakReference<T> Animatable { get; }
        public float ElapsedTime { get; private set; }
        
        public bool IsPaused { get; private set; }
        public bool IsLooping { get; set; }

        public float TimeOffset
        {
            set => ElapsedTime = value;
        }

        public IAnimation<T> Animation
        {
            get => _animation;
            set
            {
                Stop();

                _animation = value;
                _stepsToRead.Clear();

                if (_animation != null)
                {
                    IsLooping = _animation.Loop;
                    _stepsToRead.AddRange(_animation);
                }
                else
                    IsLooping = false;
            }
        }

        public bool HasEnded => ElapsedTime > Animation.Duration;
        public event Action Ended;

        public AnimationPlayer(T animatable)
        {
            Animatable = new WeakReference<T>(animatable);

            _stepsToRead = new List<AnimationTransition<T>>();
            IsPaused = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (Animation == null)
                return;

            if (IsPaused)
                return;

            if (!_stepsToRead.Any())
                return;

            ElapsedTime += elapsedTime.GetDelta(this);

            bool endLoop;
            do
            {
                endLoop = false;

                var stepsToRemove = new List<AnimationTransition<T>>();
                foreach (AnimationTransition<T> step in _stepsToRead)
                {
                    if (ElapsedTime < step.Begin)
                        break;

                    float advance = (ElapsedTime - step.Begin) / (step.End - step.Begin);
                    if (advance > 1f)
                        advance = 1f;

                    if (Animatable.TryGetTarget(out T animatable))
                        step.Apply(ref animatable, advance);

                    if (ElapsedTime > step.End)
                        stepsToRemove.Add(step);
                }

                foreach (AnimationTransition<T> step in stepsToRemove)
                    _stepsToRead.Remove(step);

                if (_stepsToRead.Count == 0)
                {
                    if (IsLooping)
                    {
                        _stepsToRead.AddRange(Animation);
                        ElapsedTime -= Animation.Duration;
                        endLoop = true;
                    }

                    Ended?.Invoke();
                }
            } while (endLoop && ElapsedTime > 0);
        }

        public void Play()
        {
            IsPaused = false;
            ElapsedTime = 0;
        }

        public void Stop()
        {
            IsPaused = true;
            ElapsedTime = 0;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }
    }
}