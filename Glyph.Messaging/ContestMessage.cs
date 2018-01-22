using System;
using System.Collections.Generic;

namespace Glyph.Messaging
{
    public class ContestMessage<T> : OpenMessage
    {
        private bool _anyCandidates;
        private readonly Comparison<T> _comparison;
        public T BestParticipant { get; private set; }

        public ContestMessage(IComparer<T> comparer)
        {
            _comparison = comparer.Compare;
        }

        public ContestMessage(Comparison<T> comparison)
        {
            _comparison = comparison;
        }
        
        public void InvolveParticipant(T participant)
        {
            if (_anyCandidates && _comparison(participant, BestParticipant) <= 0)
                return;

            BestParticipant = participant;
            _anyCandidates = true;
        }
    }

    public class ContestMessage<T, TKey> : ContestMessage<T>
    {
        public ContestMessage(Func<T, TKey> keySelector, IComparer<TKey> comparer)
            : base((x, y) => comparer.Compare(keySelector(x), keySelector(y)))
        {
        }

        public ContestMessage(Func<T, TKey> keySelector, Comparison<TKey> comparison)
            : base((x, y) => comparison(keySelector(x), keySelector(y)))
        {
        }
    }
}