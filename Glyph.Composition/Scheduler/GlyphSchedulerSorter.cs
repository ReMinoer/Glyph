using System.Collections.Generic;
using Diese.Injection;

namespace Glyph.Composition.Scheduler
{
    public class GlyphSchedulerSorter<TInterface, TDelegate> : IGlyphSchedulerAssigner
        where TInterface : class, IGlyphComponent
    {
        private readonly IDependencyInjector _injector;
        private readonly IDictionary<object, IGlyphSchedulerAssigner> _schedulers;
        public object DefaultKey { get; set; }

        public IGlyphSchedulerAssigner this[object key]
        {
            get { return _schedulers[key]; }
        }

        public GlyphSchedulerSorter(IDependencyInjector injector)
        {
            _injector = injector;
            _schedulers = new Dictionary<object, IGlyphSchedulerAssigner>();
        }

        public GlyphScheduler<TInterface, TDelegate> Add(object key)
        {
            var scheduler = _injector.Resolve<GlyphScheduler<TInterface, TDelegate>>();
            _schedulers.Add(key, scheduler);

            if (DefaultKey == null)
                DefaultKey = key;

            return scheduler;
        }

        public void StartBatch()
        {
            foreach (IGlyphSchedulerAssigner assigner in _schedulers.Values)
                assigner.StartBatch();
        }

        public void EndBatch()
        {
            foreach (IGlyphSchedulerAssigner assigner in _schedulers.Values)
                assigner.EndBatch();
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