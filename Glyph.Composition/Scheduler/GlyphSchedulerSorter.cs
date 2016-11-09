using System;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Diese.Injection;

namespace Glyph.Composition.Scheduler
{
    public class GlyphSchedulerSorter<TInterface, TDelegate> : IGlyphSchedulerAssigner
        where TInterface : class, IGlyphComponent
    {
        private readonly IDependencyInjector _injector;
        private readonly IDictionary<object, IGlyphSchedulerAssigner> _schedulers;
        public object DefaultKey { get; set; }
        public bool IsBatching => _schedulers.Values.Any(x => x.IsBatching);
        int IBatchTree.CurrentDepth => _schedulers.Values.Max(x => x.CurrentDepth);

        public GlyphSchedulerSorter(IDependencyInjector injector)
        {
            _injector = injector;
            _schedulers = new Dictionary<object, IGlyphSchedulerAssigner>();
        }

        public IGlyphSchedulerAssigner this[object key]
        {
            get { return _schedulers[key]; }
        }

        public GlyphScheduler<TInterface, TDelegate> Add(object key)
        {
            var scheduler = _injector.Resolve<GlyphScheduler<TInterface, TDelegate>>();
            _schedulers.Add(key, scheduler);

            if (DefaultKey == null)
                DefaultKey = key;

            return scheduler;
        }

        public IDisposable BeginBatch()
        {
            return new DisposableCollection(_schedulers.Values.Select(x => x.BeginBatch()));
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

        //public IState<TValue> GetState<TValue>()
        //{
        //    return _statableModel.GetState<TValue>();
        //}

        //public void SetState<TValue>(IState<TValue> state)
        //{
        //    _statableModel.SetState(state);

        //    foreach (IGlyphSchedulerAssigner scheduler in _schedulers.Values)
        //        scheduler.SetState(state);
        //}

        //public IState<TValue> AddState<TValue>()
        //{
        //    IState<TValue> state = _statableModel.AddState<TValue>();

        //    foreach (IGlyphSchedulerAssigner scheduler in _schedulers.Values)
        //        scheduler.AddState(state);

        //    return state;
        //}

        //public void AddState<TValue>(IState<TValue> state)
        //{
        //    _statableModel.AddState(state);

        //    foreach (IGlyphSchedulerAssigner scheduler in _schedulers.Values)
        //        scheduler.AddState(state);
        //}

        //public bool RemoveState<TValue>()
        //{
        //    foreach (IGlyphSchedulerAssigner scheduler in _schedulers.Values)
        //        scheduler.RemoveState<TValue>();

        //    return _statableModel.RemoveState<TValue>();
        //}
    }
}