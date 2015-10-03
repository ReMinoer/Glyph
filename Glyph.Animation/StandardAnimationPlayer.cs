using System.Collections.Generic;
using Glyph.Composition;

namespace Glyph.Animation
{
    public class StandardAnimationPlayer<T> : GlyphComponent, IAnimationPlayer
        where T : class
    {
        public T Animatable { get; private set; }
        public float ElapsedTime { get; private set; }
        private readonly List<AnimationStep<T>> _stepsToRead;
        private StandardAnimation<T> _animation;

        public StandardAnimation<T> Animation
        {
            get { return _animation; }
            set
            {
                Stop();
                _stepsToRead.Clear();

                if (_animation == null)
                    return;

                _animation = value;
                _stepsToRead.AddRange(_animation);
            }
        }

        public bool Ended
        {
            get { return ElapsedTime > Animation.Duration; }
        }

        public bool Paused { get; private set; }

        public StandardAnimationPlayer(T animatable)
        {
            Animatable = animatable;

            _stepsToRead = new List<AnimationStep<T>>();
            Paused = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (Animation == null || Animatable == null)
                return;

            if (Paused)
                return;

            ElapsedTime += (float)elapsedTime.GameTime.ElapsedGameTime.TotalSeconds;

            var stepsToRemove = new List<AnimationStep<T>>();
            foreach (AnimationStep<T> step in _stepsToRead)
            {
                if (ElapsedTime < step.Begin)
                    break;

                float advance = (ElapsedTime - step.Begin) / (step.End - step.Begin);
                if (advance > 1f)
                    advance = 1f;

                step.Apply(Animatable, advance);

                if (ElapsedTime > step.End)
                    stepsToRemove.Add(step);
            }

            foreach (AnimationStep<T> step in stepsToRemove)
                _stepsToRead.Remove(step);
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
            Animation = null;
            ElapsedTime = 0;
            Paused = true;
        }

        public void SetTimeOffset(float offset)
        {
            ElapsedTime = offset;
        }
    }
}