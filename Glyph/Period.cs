using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class Period
    {
        public float Interval { get; set; }

        [XmlIgnore]
        public float ElapsedTime { get; private set; }

        [XmlIgnore]
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

            while (ElapsedTime > Interval && Math.Abs(Interval) > float.Epsilon)
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

            while (ElapsedTime > Interval && Math.Abs(Interval) > float.Epsilon)
            {
                ElapsedTime -= Interval;
                loop++;
            }

            IsEnd = true;
            return true;
        }

        public bool Update(float elapsedMilliseconds, out float[] times)
        {
            int loop;
            if (Update(elapsedMilliseconds, out loop))
            {
                times = new float[0];
                return false;
            }

            times = new float[loop];
            times[0] = Interval - ElapsedTime;

            for (int i = 1; i < loop; i++)
                times[i] = times[0] + i * Interval;

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

        public bool Update(GameTime gameTime, out float[] times)
        {
            return Update((float)gameTime.ElapsedGameTime.TotalMilliseconds, out times);
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