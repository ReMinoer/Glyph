using System;
using System.Collections.Generic;
using System.Linq;
using Diese;

namespace Glyph.Core.Scheduler.Base
{
    public abstract class SchedulerBase<TController, T> : BatchTree, IScheduler<TController, T>
    {
        internal readonly IDictionary<T, SchedulerGraph<T>.Vertex> ItemsVertex;
        internal readonly SchedulerGraph<T> SchedulerGraph;
        private readonly TopologicalOrderVisitor<T> _topologicalOrderVisitor;

        public IEnumerable<T> Planning
        {
            get
            {
                if (IsBatching)
                    throw new InvalidOperationException(
                        "Dependency graph is currently in batch mode ! Call BatchEnd() to finish.");

                return _topologicalOrderVisitor.Result;
            }
        }

        protected SchedulerBase()
        {
            ItemsVertex = new Dictionary<T, SchedulerGraph<T>.Vertex>();

            SchedulerGraph = new SchedulerGraph<T>();
            _topologicalOrderVisitor = new TopologicalOrderVisitor<T>();
        }

        public virtual TController Plan(T item)
        {
            SchedulerGraph<T>.Vertex vertex;
            ItemsVertex.TryGetValue(item, out vertex);

            if (vertex == null)
            {
                AddItemVertex(item);
                Refresh();
            }

            return CreateController(vertex);
        }

        protected abstract TController CreateController(SchedulerGraph<T>.Vertex vertex);

        public virtual void Unplan(T item)
        {
            SchedulerGraph.ClearEdges(ItemsVertex[item]);
            Refresh();
        }

        public void ApplyProfile(ISchedulerProfile schedulerProfile)
        {
            SchedulerGraph<T>.Vertex previous = null;
            foreach (Predicate<object> predicate in schedulerProfile)
            {
                var vertex = new SchedulerGraph<T>.Vertex(predicate);
                SchedulerGraph.AddVertex(vertex);

                if (previous != null)
                {
                    var edge = new SchedulerGraph<T>.Edge();
                    SchedulerGraph.AddEdge(vertex, previous, edge);
                }

                previous = vertex;
            }

            Refresh();
        }

        protected override void OnBatchEnded()
        {
            Refresh();
        }

        internal void Add(T item)
        {
            if (ItemsVertex.ContainsKey(item))
                return;

            SchedulerGraph<T>.Vertex vertex = SchedulerGraph.Vertices.FirstOrDefault(x => x.Predicate(item));
            if (vertex != null)
                vertex.Items.Add(item);
            else
                AddItemVertex(item);

            Refresh();
        }

        internal void Remove(T item)
        {
            SchedulerGraph<T>.Vertex vertex;
            ItemsVertex.TryGetValue(item, out vertex);

            if (vertex != null)
            {
                SchedulerGraph.RemoveVertex(vertex);
                Refresh();
            }
        }

        internal void Clear()
        {
            SchedulerGraph.ClearVertices();
            Refresh();
        }

        internal SchedulerGraph<T>.Vertex AddItemVertex(T item)
        {
            var vertex = new SchedulerGraph<T>.Vertex(item);
            SchedulerGraph.AddVertex(vertex);
            ItemsVertex.Add(item, vertex);

            return vertex;
        }

        internal void Refresh()
        {
            if (IsBatching)
                return;

            _topologicalOrderVisitor.Process(SchedulerGraph);
        }
    }
}