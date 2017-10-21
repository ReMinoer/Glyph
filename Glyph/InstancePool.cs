using System;
using System.Collections.Concurrent;

namespace Glyph
{
    internal class InstancePool<T>
    {
        private readonly Func<T> _factory;
        private readonly ConcurrentBag<T> _concurrentBag = new ConcurrentBag<T>();

        public InstancePool(Func<T> factory)
        {
            _factory = factory;
        }

        public T Get()
        {
            if (!_concurrentBag.TryTake(out T item))
                item = _factory();
            return item;
        }

        public void Recondition(T usedSubject)
        {
            _concurrentBag.Add(usedSubject);
        }
    }
}