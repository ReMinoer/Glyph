using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Scheduler.Base
{
    public class Scheduler<T> : IScheduler<T>
    {
        private readonly DependencyGraph<T> _dependencyGraph;
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
            _dependencyGraph = new DependencyGraph<T>();
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
            DependencyGraph<T>.Vertex vertex = GetVertex(item);

            if (vertex == null)
            {
                vertex = new DependencyGraph<T>.Vertex(item);
                _dependencyGraph.AddVertex(vertex);
            }

            Refresh();

            return new SchedulerController(this, vertex);
        }

        public virtual void Unplan(T item)
        {
            _dependencyGraph.ClearEdges(_dependencyGraph.First(x => x.Item.Equals(item)));
            Refresh();
        }

        protected DependencyGraph<T>.Vertex GetVertex(T item)
        {
            return _dependencyGraph.FirstOrDefault(x => x.Item.Equals(item));
        }

        internal void Add(T item)
        {
            if (!_dependencyGraph.Any(x => x.Item.Equals(item)))
                _dependencyGraph.AddVertex(new DependencyGraph<T>.Vertex(item));
        }

        internal void Remove(T item)
        {
            DependencyGraph<T>.Vertex vertice = _dependencyGraph.FirstOrDefault(x => x.Item.Equals(item));
            if (vertice != null)
                _dependencyGraph.RemoveVertex(vertice);
        }

        internal void Clear()
        {
            _dependencyGraph.ClearVertices();
        }

        private void Refresh()
        {
            if (_batchMode)
                return;

            _topologicalOrderVisitor.Process(_dependencyGraph);
        }

        protected class SchedulerController : ISchedulerController<T>
        {
            protected readonly DependencyGraph<T> DependencyGraph;
            private readonly Scheduler<T> _scheduler;
            private readonly DependencyGraph<T>.Vertex _vertex;

            public SchedulerController(Scheduler<T> scheduler, DependencyGraph<T>.Vertex vertex)
            {
                _scheduler = scheduler;
                _vertex = vertex;

                DependencyGraph = _scheduler._dependencyGraph;
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
                DependencyGraph<T>.Vertex otherVertex = _scheduler.GetVertex(dependent);

                if (otherVertex == null)
                {
                    otherVertex = new DependencyGraph<T>.Vertex(dependent);
                    DependencyGraph.AddVertex(otherVertex);
                }

                var edge = new DependencyGraph<T>.Edge(otherVertex, _vertex);
                DependencyGraph.AddEdge(ref edge);

                _scheduler.Refresh();
            }

            public void After(T dependency)
            {
                DependencyGraph<T>.Vertex otherVertex = _scheduler.GetVertex(dependency);

                if (otherVertex == null)
                {
                    otherVertex = new DependencyGraph<T>.Vertex(dependency);
                    DependencyGraph.AddVertex(otherVertex);
                }

                var edge = new DependencyGraph<T>.Edge(_vertex, otherVertex);
                DependencyGraph.AddEdge(ref edge);

                _scheduler.Refresh();
            }
        }
    }
}