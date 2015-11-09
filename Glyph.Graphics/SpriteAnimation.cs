using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Graphics
{
    public class SpriteAnimation
    {
        public int[] Frames { get; private set; }
        public int[] Intervals { get; private set; }
        public bool Loop { get; set; }

        public int StepsCount
        {
            get { return Frames.Length; }
        }

        public int TotalTime
        {
            get { return TimeBetweenSteps(0, StepsCount - 1); }
        }

        public SpriteAnimation(int uniqueFrame)
            : this(new[] { uniqueFrame }, 1000, true)
        {
        }

        public SpriteAnimation(int uniqueFrame, int duration)
            : this(new[] { uniqueFrame }, duration, false)
        {
        }

        public SpriteAnimation(int begin, int end, int period, bool loop)
            : this(Enumerable.Range(System.Math.Min(begin, end), System.Math.Max(begin, end) - System.Math.Min(begin, end) + 1), period, loop)
        {
        }

        public SpriteAnimation(int begin, int end, IEnumerable<int> intervals, bool loop)
            : this(Enumerable.Range(System.Math.Min(begin, end), System.Math.Max(begin, end) - System.Math.Min(begin, end) + 1), intervals, loop)
        {
        }

        public SpriteAnimation(IEnumerable<int> indexes, int period, bool loop)
        {
            Frames = indexes.ToArray();

            Intervals = new int[Frames.Length];
            for (int i = 0; i < Frames.Length; i++)
                Intervals[i] = period;

            Loop = loop;
        }

        public SpriteAnimation(IEnumerable<int> indexes, IEnumerable<int> intervals, bool loop)
        {
            Frames = indexes.ToArray();
            Intervals = intervals.ToArray();
            Loop = loop;
        }

        public int TimeBetweenSteps(int begin, int end)
        {
            if (begin > end || begin < 0 || begin >= StepsCount || end < 0 || end >= StepsCount)
                throw new ArgumentOutOfRangeException();

            int result = 0;
            for (int i = begin; i < end; i++)
                result += Intervals[i];
            return result;
        }

        public override string ToString()
        {
            return Frames.Length + " frame" + (Frames.Length != 1 ? "s" : "") + (Loop ? ", loop" : "");
        }
    }
}