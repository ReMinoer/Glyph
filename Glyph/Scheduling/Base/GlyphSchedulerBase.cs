using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Taskete;
using Taskete.Rules;
using Taskete.Utils;

namespace Glyph.Scheduling.Base
{
    public abstract class GlyphSchedulerBase<T, TDelegate> : IGlyphScheduler<T, TDelegate, GlyphSchedulerBase<T, TDelegate>.Controller, GlyphSchedulerBase<T, TDelegate>.Controller>
    {
        protected IScheduler<T> Scheduler { get; }

        private readonly Dictionary<TDelegate, T> _delegateDictionary = new Dictionary<TDelegate, T>();
        private readonly Func<TDelegate, T> _delegateToTaskFunc;

        protected readonly TypedGroupDictionary TypedGroups = new TypedGroupDictionary();
        public float DefaultWeight { get; set; }

        public GlyphSchedulerBase(IScheduler<T> scheduler, Func<TDelegate, T> delegateToTaskFunc)
        {
            Scheduler = scheduler;
            _delegateToTaskFunc = delegateToTaskFunc;
        }

        public Controller Plan(T task)
        {
            AddTask(task);
            return new Controller(this, new[] { task });
        }

        public Controller Plan(IEnumerable<T> tasks)
        {
            foreach (T task in tasks)
                AddTask(task);
            return new Controller(this, tasks);
        }

        public Controller Plan(TDelegate taskDelegate) => Plan(GetOrAddDelegateTask(taskDelegate));

        void IGlyphScheduler<T>.Plan(T task) => Plan(task);
        void IGlyphScheduler<T>.Plan(IEnumerable<T> tasks) => Plan(tasks);
        void IGlyphScheduler<T, TDelegate>.Plan(TDelegate taskDelegate) => Plan(taskDelegate);

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

            _delegateDictionary.Remove(taskDelegate);
        }

        public T GetDelegateTask(TDelegate taskDelegate) => _delegateDictionary[taskDelegate];

        private T GetOrAddDelegateTask(TDelegate taskDelegate)
        {
            if (_delegateDictionary.TryGetValue(taskDelegate, out T task))
                return task;

            task = _delegateToTaskFunc(taskDelegate);
            _delegateDictionary.Add(taskDelegate, task);
            return task;
        }

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

        public class Controller : ControllerBase<Controller>
        {
            public Controller(GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> controlledTasks)
                : base(glyphScheduler, controlledTasks) { }

            protected override Controller ReturnedController => this;
        }

        public abstract class ControllerBase<TController> : IGlyphSchedulerController<T, TController>
        {
            protected readonly GlyphSchedulerBase<T, TDelegate> GlyphScheduler;
            protected readonly IEnumerable<T> ControlledTasks;

            protected ControllerBase(GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> controlledTasks)
            {
                GlyphScheduler = glyphScheduler;
                ControlledTasks = controlledTasks;
            }

            protected abstract TController ReturnedController { get; }

            public TController Before(T task, float? weight = null)
            {
                AddTask(task);

                AddRule(new DependencyRule<T>(ControlledTasks, new[] { task }), weight);
                return ReturnedController;
            }

            public TController After(T task, float? weight = null)
            {
                AddTask(task);

                AddRule(new DependencyRule<T>(new[] { task }, ControlledTasks), weight);
                return ReturnedController;
            }

            public TController Before(IEnumerable<T> tasks, float? weight = null)
            {
                foreach (T task in tasks)
                    AddTask(task);

                AddRule(new DependencyRule<T>(ControlledTasks, tasks), weight);
                return ReturnedController;
            }

            public TController After(IEnumerable<T> tasks, float? weight = null)
            {
                foreach (T task in tasks)
                    AddTask(task);

                AddRule(new DependencyRule<T>(tasks, ControlledTasks), weight);
                return ReturnedController;
            }

            public TController Before(TDelegate taskDelegate, float? weight = null) => Before(GlyphScheduler.GetDelegateTask(taskDelegate), weight);
            public TController After(TDelegate taskDelegate, float? weight = null) => After(GlyphScheduler.GetDelegateTask(taskDelegate), weight);

            public TController Before(Type type, float? weight = null)
            {
                AddRule(new DependencyRule<T>(ControlledTasks, GlyphScheduler.GetOrAddTypedGroup(type)), weight);
                return ReturnedController;
            }

            public TController After(Type type, float? weight = null)
            {
                AddRule(new DependencyRule<T>(GlyphScheduler.GetOrAddTypedGroup(type), ControlledTasks), weight);
                return ReturnedController;
            }

            public TController Before<TItems>(float? weight = null) => Before(typeof(TItems), weight);
            public TController After<TItems>(float? weight = null) => After(typeof(TItems), weight);

            protected void AddTask(T task)
            {
                if (!GlyphScheduler.Scheduler.Tasks.Contains(task))
                    GlyphScheduler.Scheduler.Tasks.Add(task);
            }

            protected void AddRule(DependencyRule<T> rule, float? weight = null)
            {
                rule.Weight = weight ?? GlyphScheduler.DefaultWeight;
                GlyphScheduler.Scheduler.Rules.Add(rule);
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