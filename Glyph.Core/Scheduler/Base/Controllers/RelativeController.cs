namespace Glyph.Core.Scheduler.Base.Controllers
{
    public class RelativeController<TController, T> : IRelativeController<RelativeController<TController, T>, T>
    {
        private readonly SchedulerBase<TController, T> _scheduler;
        private readonly SchedulerGraph<T>.Vertex _vertex;

        public RelativeController(SchedulerBase<TController, T> scheduler, SchedulerGraph<T>.Vertex vertex)
        {
            _scheduler = scheduler;
            _vertex = vertex;
        }

        public RelativeController<TController, T> Before(T dependent)
        {
            SchedulerGraph<T>.Vertex otherVertex;
            _scheduler.ItemsVertex.TryGetValue(dependent, out otherVertex);

            if (otherVertex == null)
                otherVertex = _scheduler.AddItemVertex(dependent);

            var edge = new SchedulerGraph<T>.Edge();
            _scheduler.SchedulerGraph.AddEdge(otherVertex, _vertex, edge);

            _scheduler.Refresh();
            return this;
        }

        public RelativeController<TController, T> After(T dependency)
        {
            SchedulerGraph<T>.Vertex otherVertex;
            _scheduler.ItemsVertex.TryGetValue(dependency, out otherVertex);

            if (otherVertex == null)
                otherVertex = _scheduler.AddItemVertex(dependency);

            var edge = new SchedulerGraph<T>.Edge();
            _scheduler.SchedulerGraph.AddEdge(_vertex, otherVertex, edge);

            _scheduler.Refresh();
            return this;
        }

        void IRelativeController<T>.After(T item)
        {
            After(item);
        }

        void IRelativeController<T>.Before(T item)
        {
            Before(item);
        }
    }
}