using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;

namespace Glyph.Graphics
{
    [SinglePerParent]
    public class SpriteAnimator : GlyphComponent, IEnableable, IUpdate
    {
        private readonly ISpriteSheet _spriteSheet;
        private readonly Period _period;
        private readonly Queue<object> _keysQueue;
        public bool Enabled { get; set; }
        public bool Ended { get; private set; }
        public Dictionary<object, SpriteAnimation> Animations { get; private set; }
        public SpriteAnimation CurrentAnimation { get; private set; }
        public object CurrentKey { get; private set; }
        public int CurrentStep { get; private set; }

        public IReadOnlyCollection<object> KeysQueue
        {
            get { return _keysQueue.ToArray(); }
        }

        public SpriteAnimator(ISpriteSheet spriteSheet)
        {
            _spriteSheet = spriteSheet;

            Animations = new Dictionary<object, SpriteAnimation>();
            _keysQueue = new Queue<object>();
            _period = new Period();

            Enabled = true;
        }

        public void ChangeAnimation(object key, SpriteAnimatorTransition transition = SpriteAnimatorTransition.Instant)
        {
            if (!Animations.ContainsKey(key))
                throw new KeyNotFoundException();

            if (key.Equals(CurrentKey))
                return;

            switch (transition)
            {
                case SpriteAnimatorTransition.Instant:

                    ChangeAnimationInstant(key);

                    break;
                case SpriteAnimatorTransition.Queued:

                    if (CurrentAnimation == null)
                        ChangeAnimationInstant(key);
                    else
                        _keysQueue.Enqueue(key);

                    break;
            }
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled || Ended)
                return;

            if (CurrentAnimation == null)
                return;

            if (!_period.Update(elapsedTime.GameTime))
                return;

            CurrentStep++;

            if (CurrentStep >= CurrentAnimation.StepsCount)
            {
                if (_keysQueue.Any())
                    ChangeAnimationInstant(_keysQueue.Dequeue());
                else if (CurrentAnimation.Loop)
                    CurrentStep = 0;
                else
                {
                    CurrentStep--;
                    Ended = true;
                    return;
                }
            }

            RefreshStep();
        }

        private void ChangeAnimationInstant(object key)
        {
            CurrentKey = key;
            CurrentAnimation = Animations[key];

            CurrentStep = 0;
            RefreshStep();

            Ended = false;
        }

        private void RefreshStep()
        {
            _spriteSheet.CurrentFrame = CurrentAnimation.Frames[CurrentStep];
            _period.Interval = CurrentAnimation.Intervals[CurrentStep];
        }
    }
}