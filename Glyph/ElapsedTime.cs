using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class ElapsedTime
    {
        static private ElapsedTime _instance;
        private float _scaleAtPause;
        public GameTime GameTime { get; private set; }
        public TimeSpan Total { get; set; }
        public float Delta { get; set; }
        public float Scale { get; set; }
        public TimeSpan UnscaledTotal { get; set; }
        public float UnscaledDelta { get; set; }

        static public ElapsedTime Instance
        {
            get { return _instance ?? (_instance = new ElapsedTime()); }
        }

        protected ElapsedTime()
        {
            Scale = 1f;
        }

        public void Refresh(GameTime gameTime)
        {
            GameTime = gameTime;

            UnscaledTotal = gameTime.TotalGameTime;
            UnscaledDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TimeSpan elapsedTimeSpan;
            if (Scale > 1 - float.Epsilon && Scale < 1 + float.Epsilon)
                elapsedTimeSpan = gameTime.ElapsedGameTime;
            else
                elapsedTimeSpan = new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks * Scale));

            Total = Total.Add(elapsedTimeSpan);
            Delta = UnscaledDelta * Scale;
        }

        public float GetDelta(ITimeUnscalable unscalable)
        {
            return unscalable.UseUnscaledTime ? UnscaledDelta : Delta;
        }

        public void Pause()
        {
            _scaleAtPause = Scale;
            Scale = 0;
        }

        public void Resume()
        {
            Scale = _scaleAtPause;
        }
    }
}