using System;

namespace Glyph.Transition
{
    public class StepsFunction : ITimingFunction
    {
        public int Steps { get; private set; }
        public bool IsStartInclude { get; private set; }

        public float Interval
        {
            get { return 1f / Steps; }
        }

        public StepsFunction(int steps, bool startInclude)
        {
            Steps = steps;
            IsStartInclude = startInclude;
        }

        public float GetValue(float t)
        {
            float a = t / Interval;
            return IsStartInclude ? (float)Math.Floor(a) * Interval : (float)Math.Ceiling(a) * Interval;
        }
    }
}