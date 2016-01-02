using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class ElapsedTime
    {
        static private ElapsedTime _instance;

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
            Delta += UnscaledDelta * Scale;
        }

        public float GetDelta(ITimeUnscalable unscalable)
        {
            return unscalable.UseUnscaledTime ? UnscaledDelta : Delta;
        }
    }
}