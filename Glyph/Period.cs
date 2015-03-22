using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class Period
    {
        public int Interval { get; set; }
        public float ElapsedTime { get; private set; }
        public bool IsEnd { get; private set; }

        public float TimeLeft
        {
            get { return Interval - ElapsedTime; }
        }

        public Period()
            : this(1000)
        {
        }

        public Period(int inter)
        {
            ElapsedTime = 0;
            Interval = inter;
            IsEnd = false;
        }

        public void Init()
        {
            ElapsedTime = 0;
            IsEnd = false;
        }

        public void InitFromEnd()
        {
            ElapsedTime = Interval + 1;
            IsEnd = true;
        }

        public bool Update(float elapsedMilliseconds)
        {
            ElapsedTime += elapsedMilliseconds;

            if (!(ElapsedTime > Interval))
                return false;

            while (ElapsedTime > Interval && Interval != 0)
                ElapsedTime -= Interval;

            IsEnd = true;
            return true;
        }

        public bool Update(float elapsedMilliseconds, out int loop)
        {
            ElapsedTime += elapsedMilliseconds;

            loop = 0;

            if (!(ElapsedTime > Interval))
                return false;

            while (ElapsedTime > Interval && Interval != 0)
            {
                ElapsedTime -= Interval;
                loop++;
            }

            IsEnd = true;
            return true;
        }

        public bool Update(GameTime gameTime)
        {
            return Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        public bool Update(GameTime gameTime, out int loop)
        {
            return Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, out loop);
        }

        static public Period Parse(string s)
        {
            return new Period(Int32.Parse(s));
        }

        public override string ToString()
        {
            return "Period : " + Interval + "ms";
        }
    }
}