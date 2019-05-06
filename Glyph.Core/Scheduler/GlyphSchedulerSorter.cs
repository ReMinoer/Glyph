using System;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Niddle;
using Glyph.Composition;

namespace Glyph.Core.Scheduler
{
    public class GlyphSchedulerSorter<TInterface, TDelegate> : IGlyphSchedulerAssigner
        where TInterface : class, IGlyphComponent
    {
        private readonly IDependencyResolver _resolver;
        private readonly IDictionary<object, IGlyphSchedulerAssigner> _schedulers;
        public object DefaultKey { get; set; }
        public bool IsBatching => _schedulers.Values.Any(x => x.IsBatching);
        int IBatchTree.BatchDepth => _schedulers.Values.Max(x => x.BatchDepth);

        public GlyphSchedulerSorter(IDependencyResolver resolver)
        {
            _resolver = resolver;
            _schedulers = new Dictionary<object, IGlyphSchedulerAssigner>();
        }

        public IGlyphSchedulerAssigner this[object key]
        {
            get { return _schedulers[key]; }
        }

        public GlyphScheduler<TInterface, TDelegate> Add(object key)
        {
            var scheduler = _resolver.Resolve<GlyphScheduler<TInterface, TDelegate>>();
            _schedulers.Add(key, scheduler);

            if (DefaultKey == null)
                DefaultKey = key;

            return scheduler;
        }

        public IDisposable Batch()
        {
            return new DisposableCollection(_schedulers.Values.Select(x => x.Batch()));
        }

        public void BeginBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers.Values)
                scheduler.BeginBatch();
        }

        public void EndBatch()
        {
            foreach (IGlyphSchedulerAssigner scheduler in _schedulers.Values)
                scheduler.EndBatch();
        }

        public void ReassignComponent(IGlyphComponent component, object assignerKey)
        {
            RemoveComponent(component);
            _schedulers[assignerKey].AssignComponent(component);
        }

        public void ReassignComponent(GlyphObject glyphObject, object assignerKey)
        {
            RemoveComponent(glyphObject);
            _schedulers[assignerKey].AssignComponent(glyphObject);
        }

        public void AssignComponent(IGlyphComponent component)
        {
            _schedulers[DefaultKey].AssignComponent(component);
        }

        public void AssignComponent(GlyphObject glyphObject)
        {
            _schedulers[DefaultKey].AssignComponent(glyphObject);
        }

        public void RemoveComponent(IGlyphComponent component)
        {
            foreach (IGlyphSchedulerAssigner assigner in _schedulers.Values)
                assigner.RemoveComponent(component);
        }

        public void RemoveComponent(GlyphObject glyphObject)
        {
            foreach (IGlyphSchedulerAssigner assigner in _schedulers.Values)
                assigner.RemoveComponent(glyphObject);
        }

        public void ClearComponents()
        {
            foreach (IGlyphSchedulerAssigner assigner in _schedulers.Values)
                assigner.ClearComponents();
        }
    }
}