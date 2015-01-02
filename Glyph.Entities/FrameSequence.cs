using System.Collections.Generic;
using System.Linq;

namespace Glyph.Entities
{
    public class FrameSequence
    {
        public List<int> Frames { get; set; }

        internal FrameSequence()
        {
            Frames = new List<int>();
        }

        public FrameSequence(int start)
            : this()
        {
            Frames.Add(start);
        }

        public FrameSequence(params int[] sequence)
            : this()
        {
            Frames.AddRange(sequence);
        }

        public FrameSequence(int startFrame, int endFrame)
            : this()
        {
            Frames.Add(startFrame);
            GoTo(endFrame);
        }

        static public FrameSequence Linear(params int[] linearPoints)
        {
            if (!linearPoints.Any())
                return new FrameSequence();

            var result = new FrameSequence(linearPoints[0]);
            for (int i = 1; i < linearPoints.Length; i++)
                result = result.GoTo(linearPoints[i]);

            return result;
        }

        public FrameSequence Add(int nextFrame)
        {
            Frames.Add(nextFrame);
            return this;
        }

        public FrameSequence GoTo(int nextFrame)
        {
            int origin = Frames.Last();
            int increment = -(origin.CompareTo(nextFrame));

            for (int i = origin + increment; i != nextFrame + increment; i += increment)
                Frames.Add(i);

            return this;
        }

        public FrameSequence ReverseToEnd(bool loop = false, int endIndex = 0)
        {
            int initialCount = Frames.Count;
            for (int i = initialCount - 2; i >= endIndex + (loop ? 1 : 0); i--)
                Frames.Add(Frames[i]);

            return this;
        }

        public FrameSequence Shift(int startIndex)
        {
            var temp = new int[Frames.Count];

            for (int i = 0; i < Frames.Count; i++)
                temp[(i + startIndex) % Frames.Count] = Frames[i];

            Frames = temp.ToList();
            return this;
        }

        static public implicit operator int[](FrameSequence x)
        {
            return x.Frames.ToArray();
        }

        static public implicit operator FrameSequence(int[] x)
        {
            return new FrameSequence(x);
        }
    }
}