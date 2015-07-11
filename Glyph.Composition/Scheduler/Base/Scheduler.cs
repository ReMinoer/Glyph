using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public class Scheduler<T> : IScheduler<T>
    {
        protected readonly IDependencyGraph<T> DependencyGraph;

        public IEnumerable<T> TopologicalOrder
        {
            get { return DependencyGraph.TopologicalOrder; }
        }

        public Scheduler()
        {
            DependencyGraph = new DependencyGraph<T>();
        }

        public void BatchStart()
        {
            DependencyGraph.BatchStart();
        }

        public void BatchEnd()
        {
            DependencyGraph.BatchEnd();
        }

        public virtual ISchedulerController<T> Plan(T item)
        {
            if (!DependencyGraph.ContainsItem(item))
                DependencyGraph.AddItem(item);

            return new SchedulerController(DependencyGraph, item);
        }

        public virtual void Unplan(T item)
        {
            DependencyGraph.ClearDependencies(item);
        }

        protected class SchedulerController : ISchedulerController<T>
        {
            protected readonly IDependencyGraph<T> DependencyGraph;
            private readonly T _item;

            public SchedulerController(IDependencyGraph<T> dependencyGraph, T item)
            {
                DependencyGraph = dependencyGraph;
                _item = item;
            }

            public void AtStart()
            {
                DependencyGraph.SetPriority(_item, Priority.High);
            }

            public void AtEnd()
            {
                DependencyGraph.SetPriority(_item, Priority.Low);
            }

            public void Before(T dependent)
            {
                if (!DependencyGraph.ContainsItem(dependent))
                    DependencyGraph.AddItem(dependent);

                DependencyGraph.AddDependency(dependent, _item);
            }

            public void After(T dependency)
            {
                if (!DependencyGraph.ContainsItem(dependency))
                    DependencyGraph.AddItem(dependency);

                DependencyGraph.AddDependency(_item, dependency);
            }
        }
    }
}