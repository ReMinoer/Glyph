using System;
using System.Collections.Generic;

namespace Glyph.Messaging
{
    public class ContestMessage<T> : OpenMessage
    {
        private bool _anyCandidates;
        private readonly Comparison<T> _comparison;
        private readonly Predicate<T> _predicate;

        public T BestParticipant { get; private set; }

        public ContestMessage(IComparer<T> comparer, Predicate<T> predicate = null)
            : this(comparer.Compare, predicate)
        {
        }

        public ContestMessage(Comparison<T> comparison, Predicate<T> predicate = null)
        {
            _comparison = comparison;
            _predicate = predicate;
        }
        
        public void InvolveParticipant(T participant)
        {
            if (!_predicate?.Invoke(participant) ?? false)
                return;

            if (_anyCandidates && _comparison(participant, BestParticipant) <= 0)
                return;

            BestParticipant = participant;
            _anyCandidates = true;
        }
    }

    public class ContestMessage<T, TKey> : ContestMessage<T>
    {
        public ContestMessage(Func<T, TKey> keySelector, IComparer<TKey> comparer, Predicate<T> predicate = null)
            : base((x, y) => comparer.Compare(keySelector(x), keySelector(y)), predicate)
        {
        }

        public ContestMessage(Func<T, TKey> keySelector, Comparison<TKey> comparison, Predicate<T> predicate = null)
            : base((x, y) => comparison(keySelector(x), keySelector(y)), predicate)
        {
        }
    }
}