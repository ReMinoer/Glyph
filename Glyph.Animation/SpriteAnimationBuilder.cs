using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Glyph.Animation
{
    public class SpriteAnimationBuilder : IAnimationBuilder<int>
    {
        public int[] Frames { get; private set; }
        public float[] Intervals { get; private set; }
        public bool Loop { get; set; }

        public int StepsCount
        {
            get { return Frames.Length; }
        }

        public float TotalTime
        {
            get { return TimeBetweenSteps(0, StepsCount - 1); }
        }

        public SpriteAnimationBuilder(int uniqueFrame)
            : this(new[] { uniqueFrame }, 1000, true)
        {
        }

        public SpriteAnimationBuilder(int uniqueFrame, float duration)
            : this(new[] { uniqueFrame }, duration, false)
        {
        }

        public SpriteAnimationBuilder(int begin, int end, float period, bool loop)
            : this(Enumerable<int>.New(begin).ThenGoTo(end), period, loop)
        {
        }

        public SpriteAnimationBuilder(int begin, int end, IEnumerable<float> intervals, bool loop)
            : this(Enumerable<int>.New(begin).ThenGoTo(end), intervals, loop)
        {
        }

        public SpriteAnimationBuilder(IEnumerable<int> indexes, float period, bool loop)
        {
            Frames = indexes.ToArray();

            Intervals = new float[Frames.Length];
            for (int i = 0; i < Frames.Length; i++)
                Intervals[i] = period / 1000;

            Loop = loop;
        }

        public SpriteAnimationBuilder(IEnumerable<int> indexes, IEnumerable<float> intervals, bool loop)
        {
            Frames = indexes.ToArray();
            Intervals = intervals.ToArray();
            Loop = loop;
        }

        public IAnimation<int> Create()
        {
            var animationBuilder = new AnimationBuilder<int>();

            float time = 0;
            for (int i = 0; i < StepsCount; i++)
            {
                int j = i;
                animationBuilder[time] = (ref int animatable, float advance) => animatable = Frames[j];
                time += Intervals[i];
            }

            animationBuilder[time] = (ref int animatable, float advance) => animatable = Frames[Frames.Length - 1];
            animationBuilder.Loop = Loop;

            return animationBuilder.Create();
        }

        public float TimeBetweenSteps(int begin, int end)
        {
            if (begin > end || begin < 0 || begin >= StepsCount || end < 0 || end >= StepsCount)
                throw new ArgumentOutOfRangeException();

            float result = 0;
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