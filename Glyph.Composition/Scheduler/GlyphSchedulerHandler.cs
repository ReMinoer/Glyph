using System.Collections.Generic;
using Diese.Injection;

namespace Glyph.Composition.Scheduler
{
    public class GlyphSchedulerHandler : IGlyphSchedulerAssigner
    {
        private readonly IDependencyInjector _injector;
        protected readonly ICollection<IGlyphSchedulerAssigner> _schedulers;

        public GlyphSchedulerHandler(IDependencyInjector injector)
        {
            _injector = injector;
            _schedulers = new List<IGlyphSchedulerAssigner>();
        }

        public GlyphScheduler<TInterface, TDelegate> Add<TInterface, TDelegate>()
            where TInterface : class, IGlyphComponent
        {
            var scheduler = _injector.Resolve<GlyphScheduler<TInterface, TDelegate>>();
            _schedulers.Add(scheduler);
            return scheduler;
        }

        public void Add(IGlyphSchedulerAssigner schedulerAssigner)
        {
            _schedulers.Add(schedulerAssigner);
        }

        public void StartBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.StartBatch();
        }

        public void EndBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.EndBatch();
        }

        public void AssignComponent(IGlyphComponent item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.AssignComponent(item);
        }

        public void AssignComponent(GlyphObject item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.AssignComponent(item);
        }

        public void RemoveComponent(IGlyphComponent item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.RemoveComponent(item);
        }

        public void RemoveComponent(GlyphObject item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.RemoveComponent(item);
        }

        public void ClearComponents()
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers)
                scheduler.ClearComponents();
        }
    }
}