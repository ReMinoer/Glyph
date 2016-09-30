using System.Collections.Generic;
using Diese;

namespace Glyph.Composition.Scheduler.Base
{
    public interface IScheduler<T> : IBatchTree
    {
        IEnumerable<T> TopologicalOrder { get; }
        ISchedulerController<T> Plan(T item);
        void Unplan(T item);
    }
}