using System;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Glyph.Composition;

namespace Glyph.Core.Scheduler
{
    public class GlyphSchedulerHandler : IGlyphSchedulerAssigner
    {
        protected readonly ICollection<IGlyphSchedulerAssigner> Schedulers;
        public int BatchDepth => Schedulers.Max(x => x.BatchDepth);
        public bool IsBatching => Schedulers.Any(x => x.IsBatching);

        public GlyphSchedulerHandler()
        {
            Schedulers = new List<IGlyphSchedulerAssigner>();
        }

        public void AddScheduler(IGlyphSchedulerAssigner schedulerAssigner)
        {
            Schedulers.Add(schedulerAssigner);
        }

        public IDisposable Batch()
        {
            return new DisposableCollection(Schedulers.Select(x => x.Batch()));
        }

        public void BeginBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
                scheduler.BeginBatch();
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