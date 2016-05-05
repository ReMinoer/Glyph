using System.Collections.Generic;
using Diese.Injection;

namespace Glyph.Composition.Scheduler
{
    public class GlyphSchedulerHandler : IGlyphSchedulerAssigner
    {
        private readonly IDependencyInjector _injector;
        protected readonly ICollection<IGlyphSchedulerAssigner> Schedulers;

        public GlyphSchedulerHandler(IDependencyInjector injector)
        {
            _injector = injector;
            Schedulers = new List<IGlyphSchedulerAssigner>();
        }

        public GlyphScheduler<TInterface, TDelegate> Add<TInterface, TDelegate>()
            where TInterface : class, IGlyphComponent
        {
            var scheduler = _injector.Resolve<GlyphScheduler<TInterface, TDelegate>>();
            Schedulers.Add(scheduler);
            return scheduler;
        }

        public void Add(IGlyphSchedulerAssigner schedulerAssigner)
        {
            Schedulers.Add(schedulerAssigner);
        }

        public void StartBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.StartBatch();
        }

        public void EndBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.EndBatch();
        }

        public void AssignComponent(IGlyphComponent item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.AssignComponent(item);
        }

        public void AssignComponent(GlyphObject item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.AssignComponent(item);
        }

        public void RemoveComponent(IGlyphComponent item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.RemoveComponent(item);
        }

        public void RemoveComponent(GlyphObject item)
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.RemoveComponent(item);
        }

        public void ClearComponents()
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.ClearComponents();
        }
    }
}