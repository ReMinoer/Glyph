using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public interface IScheduler<T> : IBatchable
    {
        IEnumerable<T> TopologicalOrder { get; }
        ISchedulerController<T> Plan(T item);
        void Unplan(T item);
    }
}