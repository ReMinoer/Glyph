using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Math
{
    public class IntegerSequencer : IEnumerable<int>
    {
        private readonly List<int> _sequence;

        public int Count
        {
            get { return _sequence.Count; }
        }

        public int this[int index]
        {
            get { return _sequence[index]; }
        }

        internal IntegerSequencer()
        {
            _sequence = new List<int>();
        }

        public IntegerSequencer(int start)
            : this()
        {
            Then(start);
        }

        public IntegerSequencer(params int[] sequence)
            : this()
        {
            Then(sequence);
        }

        public IntegerSequencer(int startFrame, int endFrame)
            : this()
        {
            Then(startFrame);
            ThenGoTo(endFrame);
        }

        public IntegerSequencer Then(params int[] n)
        {
            _sequence.AddRange(n);
            return this;
        }

        public IntegerSequencer ThenGoTo(int n, int step = 1)
        {
            int origin = this.Last();
            int increment = -(origin.CompareTo(n)) * step;

            for (int i = origin + increment; i != n + increment; i += increment)
                Then(i);

            return this;
        }

        public IntegerSequencer ThenReverse(int endIndex)
        {
            int initialCount = Count;
            for (int i = initialCount - 2; i >= endIndex; i--)
                Then(_sequence[i]);

            return this;
        }

        public IntegerSequencer ThenReverse(bool endLoop)
        {
            return ThenReverse(endLoop ? 1 : 0);
        }

        public IntegerSequencer ThenRepeat(int times, int[] subsequence)
        {
            for (int t = 0; t < times; t++)
                for (int i = 0; i < subsequence.Length; i++)
                    Then(_sequence[i]);

            return this;
        }

        public IntegerSequencer ShiftTo(int startIndex)
        {
            var temp = new int[Count];

            for (int i = 0; i < Count; i++)
                temp[(i + startIndex) % Count] = this[i];

            Clear();
            Then(temp);
            return this;
        }

        public IntegerSequencer Repeat(int times)
        {
            var temp = new int[Count];

            for (int i = 0; i < Count; i++)
                temp[i] = this[Count - i - 1];

            Clear();
            Then(temp);
            return this;
        }

        private void Clear()
        {
            _sequence.Clear();
        }

        static public IntegerSequencer Linear(params int[] linearPoints)
        {
            if (!linearPoints.Any())
                return new IntegerSequencer();

            var result = new IntegerSequencer(linearPoints[0]);
            for (int i = 1; i < linearPoints.Length; i++)
                result = result.ThenGoTo(linearPoints[i]);

            return result;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}