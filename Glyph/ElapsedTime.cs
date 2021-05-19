using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class ElapsedTime
    {
        private bool _requestNextFrame;

        public GameTime GameTime { get; private set; }
        public bool IsPaused { get; private set; }
        public TimeSpan Total { get; private set; }
        public float Delta { get; private set; }
        public float Scale { get; set; }
        public TimeSpan UnscaledTotal { get; private set; }
        public float UnscaledDelta { get; private set; }

        public ElapsedTime()
        {
            Scale = 1f;
        }

        public void Update(GameTime gameTime)
        {
            if (IsPaused && !_requestNextFrame)
            {
                Delta = 0;
                UnscaledDelta = 0;
                return;
            }

            if (GameTime == null)
                GameTime = gameTime;

            GameTime.TotalGameTime += gameTime.ElapsedGameTime;
            GameTime.ElapsedGameTime = gameTime.ElapsedGameTime;

            UnscaledTotal += gameTime.ElapsedGameTime;
            UnscaledDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Scale.EpsilonEquals(1))
            {
                Total += gameTime.ElapsedGameTime;
                Delta = UnscaledDelta;
            }
            else
            {
                Total += new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks * Scale));
                Delta = UnscaledDelta * Scale;
            }

            _requestNextFrame = false;
        }

        public float GetDelta(ITimeUnscalable unscalable)
        {
            return unscalable.UseUnscaledTime ? UnscaledDelta : Delta;
        }

        public void Pause()
        {
            if (IsPaused)
                return;
            
            IsPaused = true;
        }

        public void Resume()
        {
            if (!IsPaused)
                return;

            IsPaused = false;
        }

        public void RequestNextFrame()
        {
            _requestNextFrame = true;
        }
    }
}