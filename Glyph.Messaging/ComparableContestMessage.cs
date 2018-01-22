using System;

namespace Glyph.Messaging
{
    public class ComparableContestMessage<T> : ContestMessage<T>
        where T : IComparable<T>
    {
        public ComparableContestMessage(bool inverseComparison = false)
            : base((x, y) => x.CompareTo(y) * (inverseComparison ? -1 : 1))
        {
        }
    }

    public class ComparableContestMessage<T, TKey> : ContestMessage<T, TKey>
        where TKey : IComparable<TKey>
    {
        public ComparableContestMessage(Func<T, TKey> keySelector, bool inverseComparison = false)
            : base(keySelector, (x, y) => x.CompareTo(y) * (inverseComparison ? -1 : 1))
        {
        }
    }
}