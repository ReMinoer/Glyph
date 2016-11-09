namespace Glyph.Composition.Scheduler.Base.Controllers
{
    public class PriorityController<TController, T> : IPriorityController<PriorityController<TController, T>>
    {
        private readonly SchedulerBase<TController, T> _scheduler;
        private readonly SchedulerGraph<T>.Vertex _vertex;

        public PriorityController(SchedulerBase<TController, T> scheduler, SchedulerGraph<T>.Vertex vertex)
        {
            _scheduler = scheduler;
            _vertex = vertex;
        }

        public PriorityController<TController, T> AtStart()
        {
            _vertex.Priority = Priority.High;
            _scheduler.Refresh();
            return this;
        }

        public PriorityController<TController, T> AtEnd()
        {
            _vertex.Priority = Priority.Low;
            _scheduler.Refresh();
            return this;
        }

        void IPriorityController.AtStart()
        {
            AtStart();
        }

        void IPriorityController.AtEnd()
        {
            AtEnd();
        }
    }
}