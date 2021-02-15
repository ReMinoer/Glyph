using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taskete;
using Taskete.Rules;
using Taskete.Schedulers;
using Taskete.Utils;

namespace Glyph.Core.Scheduler
{
    public class GlyphScheduler<T> : GlyphSchedulerBase<T>
    {
        private readonly LinearScheduler<T> _scheduler;
        public IEnumerable<T> Schedule => _scheduler.Schedule;

        public GlyphScheduler()
            : base(new LinearScheduler<T>())
        {
            _scheduler = (LinearScheduler<T>)Scheduler;
        }
    }

    public class AsyncGlyphScheduler<T, TParam> : GlyphSchedulerBase<T>
    {
        private readonly AsyncScheduler<T, TParam> _scheduler;

        public AsyncGlyphScheduler(Func<T, TParam, CancellationToken, Task> awaitableSelector)
            : base(new AsyncScheduler<T, TParam>(awaitableSelector))
        {
            _scheduler = (AsyncScheduler<T, TParam>)Scheduler;
        }

        public Task RunScheduleAsync(TParam param) => _scheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _scheduler.RunScheduleAsync(param, cancellationToken);
    }

    public abstract class GlyphSchedulerBase<T> : IGlyphScheduler<T, GlyphSchedulerBase<T>.ItemController, GlyphSchedulerBase<T>.TypeController>
    {
        protected IScheduler<T> Scheduler { get; }

        protected readonly TypedGroupDictionary TypedGroups = new TypedGroupDictionary();
        protected readonly PriorityGroupDictionary PriorityGroups = new PriorityGroupDictionary();
        public float DefaultWeight { get; set; }

        public GlyphSchedulerBase(IScheduler<T> scheduler)
        {
            PriorityGroups.Add(Priority.Low, new PriorityGroup());
            PriorityGroups.Add(Priority.Normal, new PriorityGroup());
            PriorityGroups.Add(Priority.High, new PriorityGroup());

            Scheduler = scheduler;
            scheduler.Rules.Add(DependencyRule<T>.New(PriorityGroups[Priority.High], PriorityGroups[Priority.Normal]));
            scheduler.Rules.Add(DependencyRule<T>.New(PriorityGroups[Priority.Normal], PriorityGroups[Priority.Low]));
        }

        public ItemController Plan(T task)
        {
            AddTask(task);
            return GetItemController(task);
        }

        void IGlyphScheduler<T>.Plan(T task) => AddTask(task);

        private void AddTask(T task)
        {
            if (Scheduler.Tasks.Contains(task))
                return;

            Scheduler.Tasks.Add(task);

            foreach (TypedGroup typedGroup in TypedGroups.GetAllValues(task.GetType()))
                typedGroup.Add(task);

            PriorityGroups[Priority.Normal].Add(task);
        }

        public TypeController Plan<TTasks>() => Plan(typeof(TTasks));
        public TypeController Plan(Type taskType) => GetTypeController(taskType);

        public void Unplan(T task)
        {
            foreach (PriorityGroup priorityGroup in PriorityGroups.Values)
                priorityGroup.Remove(task);
            foreach (TypedGroup typedGroup in TypedGroups.GetAllValues(task.GetType()))
                typedGroup.Remove(task);

            Scheduler.Tasks.Remove(task);
        }

        protected ItemController GetItemController(T task) => new ItemController(this, task);
        protected TypeController GetTypeController(Type taskType) => new TypeController(this, taskType);

        public class ItemController : ItemController<ItemController>
        {
            public ItemController(GlyphSchedulerBase<T> glyphScheduler, params T[] controlledTasks)
                : base(glyphScheduler, controlledTasks) { }

            protected override ItemController Controller => this;
        }

        public abstract class ItemController<TController> : ControllerBase<TController>
        {
            protected readonly T[] ControlledTasks;
            protected Priority Priority;

            public ItemController(GlyphSchedulerBase<T> glyphScheduler, params T[] controlledTasks)
                : base(glyphScheduler)
            {
                ControlledTasks = controlledTasks;
                Priority = Priority.Normal;
            }

            public override TController Before(T item, float? weight = null)
            {
                AddTask(item);
                AddRule(DependencyRule<T>.New(ControlledTasks, new[] { item }), weight);
                return Controller;
            }

            public override TController After(T item, float? weight = null)
            {
                AddTask(item);
                AddRule(DependencyRule<T>.New(new[] { item }, ControlledTasks), weight);
                return Controller;
            }

            public override TController Before(Type type, float? weight = null)
            {
                AddRule(DependencyRule<T>.New(ControlledTasks, GetOrAddTypedGroup(type)), weight);
                return Controller;
            }

            public override TController After(Type type, float? weight = null)
            {
                AddRule(DependencyRule<T>.New(GetOrAddTypedGroup(type), ControlledTasks), weight);
                return Controller;
            }

            public TController AtStart()
            {
                SwitchPriorityGroup(Priority.High);
                return Controller;
            }

            public TController AtEnd()
            {
                SwitchPriorityGroup(Priority.Low);
                return Controller;
            }

            private void SwitchPriorityGroup(Priority priority)
            {
                foreach (T controlledTask in ControlledTasks)
                    PriorityGroups[Priority].Remove(controlledTask);

                Priority = priority;

                foreach (T controlledTask in ControlledTasks)
                    PriorityGroups[Priority].Add(controlledTask);
            }
        }

        public class TypeController : TypeController<TypeController>
        {
            public TypeController(GlyphSchedulerBase<T> glyphScheduler, Type type)
                : base(glyphScheduler, type) { }

            protected override TypeController Controller => this;
        }

        public abstract class TypeController<TController> : ControllerBase<TController>
        {
            protected readonly TypedGroup ControlledTypedGroup;

            public TypeController(GlyphSchedulerBase<T> glyphScheduler, Type type)
                : base(glyphScheduler)
            {
                ControlledTypedGroup = GetOrAddTypedGroup(type);
            }

            public override TController Before(T item, float? weight = null)
            {
                AddTask(item);
                AddRule(DependencyRule<T>.New(ControlledTypedGroup, new[] { item }), weight);
                return Controller;
            }

            public override TController After(T item, float? weight = null)
            {
                AddTask(item);
                AddRule(DependencyRule<T>.New(new[] { item }, ControlledTypedGroup), weight);
                return Controller;
            }

            public override TController Before(Type type, float? weight = null)
            {
                AddRule(DependencyRule<T>.New(ControlledTypedGroup, GetOrAddTypedGroup(type)), weight);
                return Controller;
            }

            public override TController After(Type type, float? weight = null)
            {
                AddRule(DependencyRule<T>.New(GetOrAddTypedGroup(type), ControlledTypedGroup), weight);
                return Controller;
            }
        }

        public abstract class ControllerBase<TController> : IGlyphSchedulerController<T, TController>
        {
            private readonly GlyphSchedulerBase<T> _glyphScheduler;

            protected IScheduler<T> Scheduler => _glyphScheduler.Scheduler;
            protected TypedGroupDictionary TypedGroups => _glyphScheduler.TypedGroups;
            protected PriorityGroupDictionary PriorityGroups => _glyphScheduler.PriorityGroups;
            protected float DefaultWeight => _glyphScheduler.DefaultWeight;

            protected ControllerBase(GlyphSchedulerBase<T> glyphScheduler)
            {
                _glyphScheduler = glyphScheduler;
            }

            protected abstract TController Controller { get; }

            public abstract TController Before(T item, float? weight = null);
            public abstract TController After(T item, float? weight = null);
            public abstract TController Before(Type type, float? weight = null);
            public abstract TController After(Type type, float? weight = null);

            public TController Before<TItems>(float? weight = null) => Before(typeof(TItems), weight);
            public TController After<TItems>(float? weight = null) => After(typeof(TItems), weight);

            protected void AddTask(T task)
            {
                if (!Scheduler.Tasks.Contains(task))
                    Scheduler.Tasks.Add(task);
            }

            protected void AddRule(DependencyRule<T> rule, float? weight = null)
            {
                rule.Weight = weight ?? DefaultWeight;
                Scheduler.Rules.Add(rule);
            }

            protected TypedGroup GetOrAddTypedGroup(Type type)
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
        }

        public class TypedGroupDictionary : AssignableTypeDictionary<TypedGroup>
        {
        }

        public class TypedGroup : ObservableCollection<T>
        {
        }

        public enum Priority
        {
            Low,
            Normal,
            High
        }

        public class PriorityGroupDictionary : Dictionary<Priority, PriorityGroup>
        {
        }

        public class PriorityGroup : ObservableCollection<T>
        {
        }
    }
}