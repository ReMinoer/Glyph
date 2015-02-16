using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Glyph.Entities
{
    public class Animation
    {
        public int[] Frames { get; set; }
        public int[] Intervals { get; set; }
        public bool Loop { get; set; }
        [XmlIgnore]
        public bool IsEnd { get; set; }
        [XmlIgnore]
        public bool IsPause { get; set; }
        [XmlIgnore]
        public int NbFrames { get { return Frames.Length; } }
        [XmlIgnore]
        public int FrameActual { get { return Frames[_currentState]; } }
        [XmlIgnore]
        public bool CanChange { get { return Loop || IsEnd; } }
        [XmlIgnore]
        public int TotalTime { get { return TimeBetweenFrames(0, NbFrames - 1); } }
        static public Animation Default { get { return new Animation(); } }
        private int _currentState;
        private Period _period;

        public Animation()
        {
            _currentState = 0;
            Loop = false;
            IsEnd = false;
            IsPause = false;
            Frames = new[] {0};
            Intervals = new[] {0};
            _period = new Period(0);
        }

        public Animation(int[] id, int[] intervals, bool loop)
            : this()
        {
            Frames = id;
            Intervals = intervals;

            Loop = loop;
            _period = new Period(Intervals[_currentState]);
        }

        public Animation(int[] id, int period, bool loop)
            : this()
        {
            Frames = id;

            Intervals = new int[Frames.Length];
            for (int i = 0; i < Frames.Length; i++)
                Intervals[i] = period;

            Loop = loop;
            _period = new Period(Intervals[_currentState]);
        }

        public Animation(FrameSequence sequence, int period, bool loop)
            : this(sequence.Frames.ToArray(), period, loop) {}

        public Animation(FrameSequence sequence, int[] intervals, bool loop)
            : this(sequence.Frames.ToArray(), intervals, loop) {}

        public Animation(int uniqueFrame)
            : this(new[] {uniqueFrame}, 1000, true) {}

        public Animation(int uniqueFrame, int duration)
            : this(new[] {uniqueFrame}, duration, false) {}

        public Animation(int begin, int end, int period, bool loop)
            : this(FrameSequence.Linear(begin, end), period, loop) {}

        public Animation(int begin, int end, int[] intervals, bool loop)
            : this(FrameSequence.Linear(begin, end), intervals, loop) {}

        public void Initialize()
        {
            _currentState = 0;
            _period = new Period(Intervals[_currentState]);
            IsEnd = false;
            IsPause = false;
        }

        public void Update(GameTime gameTime)
        {
            if (IsPause)
                return;

            if (!_period.Update(gameTime) || IsEnd)
                return;

            _currentState++;

            if (!Loop && _currentState >= Frames.Length)
                IsEnd = true;

            _currentState %= Frames.Length;
            _period.Interval = Intervals[_currentState];
        }

        public void SetAllIntervals(int inter)
        {
            for (int i = 0; i < Frames.Length; i++)
                Intervals[i] = inter;
            _period.Interval = Intervals[_currentState];
        }

        public int TimeBetweenFrames(int begin, int end)
        {
            if (begin > end || begin < 0 || begin >= NbFrames || end < 0 || end >= NbFrames)
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