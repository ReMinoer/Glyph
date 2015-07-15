using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Scheduler.Base
{
    public class Scheduler<T> : IScheduler<T>
    {
        protected readonly IDictionary<T, SchedulerGraph<T>.Vertex> ItemsVertex;

        private readonly SchedulerGraph<T> _schedulerGraph;
        private readonly TopologicalOrderVisitor<T> _topologicalOrderVisitor;
        private bool _batchMode;

        public IEnumerable<T> TopologicalOrder
        {
            get
            {
                if (_batchMode)
                    throw new InvalidOperationException(
                        "Dependency graph is currently in batch mode ! Call BatchEnd() to finish.");

                return _topologicalOrderVisitor.Result;
            }
        }

        public Scheduler()
        {
            ItemsVertex = new Dictionary<T, SchedulerGraph<T>.Vertex>();

            _schedulerGraph = new SchedulerGraph<T>();
            _topologicalOrderVisitor = new TopologicalOrderVisitor<T>();
        }

        public void BatchStart()
        {
            _batchMode = true;
        }

        public void BatchEnd()
        {
            _batchMode = false;
            Refresh();
        }

        public virtual ISchedulerController<T> Plan(T item)
        {
            SchedulerGraph<T>.Vertex vertex;
            ItemsVertex.TryGetValue(item, out vertex);

            if (vertex == null)
            {
                AddItemVertex(item);
                Refresh();
            }

            return new SchedulerController(this, vertex);
        }

        public virtual void Unplan(T item)
        {
            _schedulerGraph.ClearEdges(ItemsVertex[item]);
            Refresh();
        }

        public void ApplyProfile(ISchedulerProfile schedulerProfile)
        {
            SchedulerGraph<T>.Vertex previous = null;
            foreach (Func<object, bool> predicate in schedulerProfile)
            {
                var vertex = new SchedulerGraph<T>.Vertex(predicate);
                _schedulerGraph.AddVertex(new SchedulerGraph<T>.Vertex(predicate));

                if (previous != null)
                {
                    var edge = new SchedulerGraph<T>.Edge(vertex, previous);
                    _schedulerGraph.AddEdge(ref edge);
                }

                previous = vertex;
            }

            Refresh();
        }

        internal void Add(T item)
        {
            if (ItemsVertex.ContainsKey(item))
                return;

            SchedulerGraph<T>.Vertex vertex = _schedulerGraph.FirstOrDefault(x => x.Predicate(item));
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
                _schedulerGraph.RemoveVertex(vertex);
                Refresh();
            }
        }

        internal void Clear()
        {
            _schedulerGraph.ClearVertices();
            Refresh();
        }

        private SchedulerGraph<T>.Vertex AddItemVertex(T item)
        {
            var vertex = new SchedulerGraph<T>.Vertex(item);
            _schedulerGraph.AddVertex(vertex);
            ItemsVertex.Add(item, vertex);

            return vertex;
        }

        private void Refresh()
        {
            if (_batchMode)
                return;

            _topologicalOrderVisitor.Process(_schedulerGraph);
        }

        protected class SchedulerController : ISchedulerController<T>
        {
            protected readonly SchedulerGraph<T> SchedulerGraph;
            private readonly Scheduler<T> _scheduler;
            private readonly SchedulerGraph<T>.Vertex _vertex;

            public SchedulerController(Scheduler<T> scheduler, SchedulerGraph<T>.Vertex vertex)
            {
                _scheduler = scheduler;
                _vertex = vertex;

                SchedulerGraph = _scheduler._schedulerGraph;
            }

            public void AtStart()
            {
                _vertex.Priority = Priority.High;
                _scheduler.Refresh();
            }

            public void AtEnd()
            {
                _vertex.Priority = Priority.Low;
                _scheduler.Refresh();
            }

            public void Before(T dependent)
            {
                SchedulerGraph<T>.Vertex otherVertex;
                _scheduler.ItemsVertex.TryGetValue(dependent, out otherVertex);

                if (otherVertex == null)
                    otherVertex = _scheduler.AddItemVertex(dependent);

                var edge = new SchedulerGraph<T>.Edge(otherVertex, _vertex);
                SchedulerGraph.AddEdge(ref edge);

                _scheduler.Refresh();
            }

            public void After(T dependency)
            {
                SchedulerGraph<T>.Vertex otherVertex;
                _scheduler.ItemsVertex.TryGetValue(dependency, out otherVertex);

                if (otherVertex == null)
                    otherVertex = _scheduler.AddItemVertex(dependency);

                var edge = new SchedulerGraph<T>.Edge(_vertex, otherVertex);
                SchedulerGraph.AddEdge(ref edge);

                _scheduler.Refresh();
            }
        }
    }
}