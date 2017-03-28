using System;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Diese.Injection;
using Glyph.Composition;

namespace Glyph.Core.Scheduler
{
    public class GlyphSchedulerHandler : IGlyphSchedulerAssigner
    {
        private readonly IDependencyInjector _injector;
        protected readonly ICollection<IGlyphSchedulerAssigner> Schedulers;
        public int CurrentDepth => Schedulers.Max(x => x.CurrentDepth);
        public bool IsBatching => Schedulers.Any(x => x.IsBatching);

        public GlyphSchedulerHandler(IDependencyInjector injector)
        {
            _injector = injector;
            Schedulers = new List<IGlyphSchedulerAssigner>();
        }

        public GlyphScheduler<TInterface, TDelegate> AddScheduler<TInterface, TDelegate>()
            where TInterface : class, IGlyphComponent
        {
            var scheduler = _injector.Resolve<GlyphScheduler<TInterface, TDelegate>>();
            Schedulers.Add(scheduler);
            return scheduler;
        }

        public void AddScheduler(IGlyphSchedulerAssigner schedulerAssigner)
        {
            Schedulers.Add(schedulerAssigner);
        }

        public IDisposable BeginBatch()
        {
            return new DisposableCollection(Schedulers.Select(x => x.BeginBatch()));
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

        //public IState<TValue> GetState<TValue>()
        //{
        //    return _statableModel.GetState<TValue>();
        //}

        //public void SetState<TValue>(IState<TValue> state)
        //{
        //    _statableModel.SetState(state);

        //    foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
        //        scheduler.SetState(state);
        //}

        //public IState<TValue> AddState<TValue>()
        //{
        //    IState<TValue> state = _statableModel.AddState<TValue>();

        //    foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
        //        scheduler.AddState(state);

        //    return state;
        //}

        //public void AddState<TValue>(IState<TValue> state)
        //{
        //    _statableModel.AddState(state);

        //    foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
        //        scheduler.AddState(state);
        //}

        //public bool RemoveState<TValue>()
        //{
        //    foreach (IGlyphSchedulerAssigner scheduler in Schedulers)
        //        scheduler.RemoveState<TValue>();

        //    return _statableModel.RemoveState<TValue>();
        //}
    }
}