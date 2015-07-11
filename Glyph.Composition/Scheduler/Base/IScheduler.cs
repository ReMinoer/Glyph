using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public interface IScheduler<T>
    {
        IEnumerable<T> TopologicalOrder { get; }
        void BatchStart();
        void BatchEnd();
        ISchedulerController<T> Plan(T item);
        void Unplan(T item);
    }
}