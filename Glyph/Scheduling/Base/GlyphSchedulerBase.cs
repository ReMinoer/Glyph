using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Taskete;
using Taskete.Rules;
using Taskete.Utils;

namespace Glyph.Scheduling.Base
{
    public abstract class GlyphSchedulerBase<T, TDelegate>
        : IGlyphDelegateScheduler<T, TDelegate, GlyphSchedulerBase<T, TDelegate>.Controller, GlyphSchedulerBase<T, TDelegate>.Controller>
        where T : class
    {
        protected IScheduler<T> Scheduler { get; }

        private readonly ConcurrentDictionary<TDelegate, T> _delegateDictionary = new ConcurrentDictionary<TDelegate, T>();
        private readonly Func<TDelegate, T> _delegateToTaskFunc;

        protected readonly TypedGroupDictionary TypedGroups = new TypedGroupDictionary();
        public float DefaultWeight { get; set; }

        public GlyphSchedulerBase(IScheduler<T> scheduler, Func<TDelegate, T> delegateToTaskFunc)
        {
            Scheduler = scheduler;
            _delegateToTaskFunc = delegateToTaskFunc;
        }

        public Controller Plan(T task) => Plan(new[] {task});
        public virtual Controller Plan(IEnumerable<T> tasks)
        {
            foreach (T task in tasks)
                AddTask(task);
            return new Controller(this, tasks);
        }

        public Controller Plan(TDelegate taskDelegate) => Plan(GetOrAddDelegateTask(taskDelegate));

        void IGlyphScheduler<T>.Plan(T task) => Plan(task);
        void IGlyphDelegateScheduler<T, TDelegate>.Plan(TDelegate taskDelegate) => Plan(taskDelegate);

        private void AddTask(T task)
        {
            if (Scheduler.Tasks.Contains(task))
                return;

            Scheduler.Tasks.Add(task);

            foreach (TypedGroup typedGroup in TypedGroups.GetAllValues(task.GetType()))
                typedGroup.Add(task);
        }

        public Controller Plan<TTasks>() => Plan(typeof(TTasks));
        public Controller Plan(Type taskType) => new Controller(this, GetOrAddTypedGroup(taskType));

        public void Unplan(T task)
        {
            foreach (TypedGroup typedGroup in TypedGroups.GetAllValues(task.GetType()))
                typedGroup.Remove(task);

            Scheduler.Tasks.Remove(task);
        }

        public void Unplan(TDelegate taskDelegate)
        {
            T task = GetDelegateTask(taskDelegate);
            Unplan(task);

            _delegateDictionary.TryRemove(taskDelegate, out _);
        }

        public T GetDelegateTask(TDelegate taskDelegate) => _delegateDictionary[taskDelegate];

        protected T GetOrAddDelegateTask(TDelegate taskDelegate) => GetOrAddTask(taskDelegate, () => _delegateToTaskFunc(taskDelegate));
        protected T GetOrAddTask(TDelegate taskDelegate, Func<T> taskFunc) => _delegateDictionary.GetOrAdd(taskDelegate, _ => taskFunc());

        public TypedGroup GetOrAddTypedGroup(Type type)
        {
            if (!TypedGroups.TryGetValue(type, out TypedGroup typedGroup))
            {
                typedGroup = new TypedGroup();
                foreach (T task in Scheduler.Tasks.Where(x => type.IsInstanceOfType(x)))
                    typedGroup.Add(task);

                TypedGroups.Add(type, typedGroup);
            }

            return typedGroup;
        }

        public class Controller : ControllerBase<Controller, T>
        {
            public Controller(GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> controlledTasks)
                : base(glyphScheduler, controlledTasks) { }

            protected override Controller ReturnedController => this;
        }

        public abstract class ControllerBase<TController, TControlled> : IGlyphSchedulerController<T, TController>
            where TControlled : class, T
        {
            protected readonly GlyphSchedulerBase<T, TDelegate> GlyphScheduler;
            protected readonly IEnumerable<TControlled> ControlledTasks;

            private DependencyRule<T> _lastRule;

            protected ControllerBase(GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<TControlled> controlledTasks)
            {
                GlyphScheduler = glyphScheduler;
                ControlledTasks = controlledTasks;
            }

            protected abstract TController ReturnedController { get; }

            public TController Before(T task)
            {
                AddTask(task);

                AddRule(new DependencyRule<T>(ControlledTasks, new[] { task }));
                return ReturnedController;
            }

            public TController After(T task)
            {
                AddTask(task);

                AddRule(new DependencyRule<T>(new[] { task }, ControlledTasks));
                return ReturnedController;
            }

            public TController Before(IEnumerable<T> tasks)
            {
                foreach (T task in tasks)
                    AddTask(task);

                AddRule(new DependencyRule<T>(ControlledTasks, tasks));
                return ReturnedController;
            }

            public TController After(IEnumerable<T> tasks)
            {
                foreach (T task in tasks)
                    AddTask(task);

                AddRule(new DependencyRule<T>(tasks, ControlledTasks));
                return ReturnedController;
            }

            public TController Before(TDelegate taskDelegate) => Before(GlyphScheduler.GetDelegateTask(taskDelegate));
            public TController After(TDelegate taskDelegate) => After(GlyphScheduler.GetDelegateTask(taskDelegate));

            public TController Before(Type type)
            {
                AddRule(new DependencyRule<T>(ControlledTasks, GlyphScheduler.GetOrAddTypedGroup(type)));
                return ReturnedController;
            }

            public TController After(Type type)
            {
                AddRule(new DependencyRule<T>(GlyphScheduler.GetOrAddTypedGroup(type), ControlledTasks));
                return ReturnedController;
            }

            public TController Before<TItems>() => Before(typeof(TItems));
            public TController After<TItems>() => After(typeof(TItems));

            public TController WithWeight(float weight)
            {
                _lastRule.Weight = weight;
                return ReturnedController;
            }

            public TController Optional()
            {
                _lastRule.MustBeApplied = false;
                return ReturnedController;
            }

            protected void AddTask(T task)
            {
                if (!GlyphScheduler.Scheduler.Tasks.Contains(task))
                    GlyphScheduler.Scheduler.Tasks.Add(task);
            }

            protected void AddRule(DependencyRule<T> rule)
            {
                rule.MustBeApplied = true;
                rule.Weight = GlyphScheduler.DefaultWeight;

                GlyphScheduler.Scheduler.Rules.Add(rule);
                _lastRule = rule;
            }
        }

        protected class TypedGroupDictionary : AssignableTypeDictionary<TypedGroup>
        {
        }

        public class TypedGroup : ObservableCollection<T>
        {
        }
    }
}