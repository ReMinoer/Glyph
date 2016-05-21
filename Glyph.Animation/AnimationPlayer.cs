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
        public WeakReference<T> Animatable { get; private set; }
        public float ElapsedTime { get; private set; }

        public bool Paused { get; private set; }
        public bool IsLooping { get; set; }

        public IAnimation<T> Animation
        {
            get { return _animation; }
            set
            {
                Stop();
                if (value == null)
                    return;

                _animation = value;
                IsLooping = _animation.Loop;
                _stepsToRead.AddRange(_animation);
            }
        }

        public bool Ended
        {
            get { return ElapsedTime > Animation.Duration; }
        }

        public AnimationPlayer(T animatable)
        {
            Animatable = new WeakReference<T>(animatable);

            _stepsToRead = new List<AnimationTransition<T>>();
            Paused = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (Animation == null)
                return;

            if (Paused)
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

                    T animatable;
                    if (Animatable.TryGetTarget(out animatable))
                        step.Apply(ref animatable, advance);

                    if (ElapsedTime > step.End)
                        stepsToRemove.Add(step);
                }

                foreach (AnimationTransition<T> step in stepsToRemove)
                    _stepsToRead.Remove(step);

                if (IsLooping && !_stepsToRead.Any())
                {
                    _stepsToRead.AddRange(Animation);
                    ElapsedTime -= Animation.Duration;
                    endLoop = true;
                }
            } while (endLoop && ElapsedTime > 0);
        }

        public void Play()
        {
            Paused = false;
            ElapsedTime = 0;
        }

        public void Stop()
        {
            _animation = null;
            _stepsToRead.Clear();

            ElapsedTime = 0;
            Paused = true;
            IsLooping = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void SetTimeOffset(float offset)
        {
            ElapsedTime = offset;
        }
    }
}