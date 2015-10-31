using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;

namespace Glyph.Animation
{
    public class StandardAnimationPlayer<T> : GlyphComponent, IAnimationPlayer
        where T : class
    {
        private readonly List<AnimationStep<T>> _stepsToRead;
        private StandardAnimation<T> _animation;
        public bool UseUnscaledTime { get; set; }
        public WeakReference<T> Animatable { get; private set; }
        public float ElapsedTime { get; private set; }

        public StandardAnimation<T> Animation
        {
            get { return _animation; }
            set
            {
                Stop();
                _stepsToRead.Clear();

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

        public bool Paused { get; private set; }
        public bool IsLooping { get; set; }

        public StandardAnimationPlayer(T animatable)
        {
            Animatable = new WeakReference<T>(animatable);

            _stepsToRead = new List<AnimationStep<T>>();
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

            do
            {
                var stepsToRemove = new List<AnimationStep<T>>();
                foreach (AnimationStep<T> step in _stepsToRead)
                {
                    if (ElapsedTime < step.Begin)
                        break;

                    float advance = (ElapsedTime - step.Begin) / (step.End - step.Begin);
                    if (advance > 1f)
                        advance = 1f;

                    T animatable;
                    if (Animatable.TryGetTarget(out animatable))
                        step.Apply(animatable, advance);

                    if (ElapsedTime > step.End)
                        stepsToRemove.Add(step);
                }

                foreach (AnimationStep<T> step in stepsToRemove)
                    _stepsToRead.Remove(step);

                if (!_stepsToRead.Any())
                {
                    _stepsToRead.AddRange(Animation);
                    ElapsedTime -= Animation.Duration;
                }

            } while (ElapsedTime > 0);
            ElapsedTime -= Animation.Duration;
        }

        public void Play()
        {
            Paused = false;
            ElapsedTime = 0;
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Stop()
        {
            _animation = null;
            ElapsedTime = 0;
            Paused = true;
        }

        public void SetTimeOffset(float offset)
        {
            ElapsedTime = offset;
        }
    }
}