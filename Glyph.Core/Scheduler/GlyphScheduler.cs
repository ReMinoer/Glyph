using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Taskete;
using Taskete.Rules;
using Taskete.Schedulers;
using Taskete.Utils;

namespace Glyph.Core.Scheduler
{
    public class InitializeScheduler : GlyphScheduler<IInitializeTask, InitializeDelegate>
    {
        public InitializeScheduler()
            : base(x => new InitializeTask(x)) { }
    }

    public class LoadContentScheduler : AsyncGlyphScheduler<ILoadContentTask, LoadContentDelegate, IContentLibrary>
    {
        public LoadContentScheduler()
            : base(x => new LoadContentTask(x), (x, contentLibrary, _) => x.LoadContent(contentLibrary)) { }
    }

    public class UpdateScheduler : GlyphScheduler<IUpdateTask, UpdateDelegate>
    {
        public UpdateScheduler()
            : base(x => new UpdateTask(x)) { }
    }

    public class DrawScheduler : GlyphScheduler<IDrawTask, DrawDelegate>
    {
        public DrawScheduler()
            : base(x => new DrawTask(x)) {}
    }

    public class DelegateTaskBase<TDelegate>
        where TDelegate : Delegate
    {
        protected readonly TDelegate TaskDelegate;
        public DelegateTaskBase(TDelegate taskDelegate) => TaskDelegate = taskDelegate;

        public override string ToString() => $"{TaskDelegate.Method.DeclaringType?.Name}.{TaskDelegate.Method.Name} - {TaskDelegate.Target}";
    }

    public class InitializeTask : DelegateTaskBase<InitializeDelegate>, IInitializeTask
    {
        public InitializeTask(InitializeDelegate taskDelegate)
            : base(taskDelegate) { }

        public void Initialize() => TaskDelegate();
    }

    public class LoadContentTask : DelegateTaskBase<LoadContentDelegate>, ILoadContentTask
    {
        public LoadContentTask(LoadContentDelegate taskDelegate)
            : base(taskDelegate) { }

        public Task LoadContent(IContentLibrary contentLibrary) => TaskDelegate(contentLibrary);
    }

    public class UpdateTask : DelegateTaskBase<UpdateDelegate>, IUpdateTask
    {
        public UpdateTask(UpdateDelegate taskDelegate)
            : base(taskDelegate) { }

        public bool Active => true;
        public void Update(ElapsedTime elapsedTime) => TaskDelegate(elapsedTime);
    }

    public class DrawTask : DelegateTaskBase<DrawDelegate>, IDrawTask
    {
        public DrawTask(DrawDelegate taskDelegate)
            : base(taskDelegate) { }

        public void Draw(IDrawer drawer) => TaskDelegate(drawer);
    }

    public class GlyphScheduler<T, TDelegate> : GlyphSchedulerBase<T, TDelegate>
    {
        private readonly LinearScheduler<T> _scheduler;
        public IEnumerable<T> Schedule => _scheduler.Schedule;

        public GlyphScheduler(Func<TDelegate, T> delegateToTaskFunc)
            : base(new LinearScheduler<T>(), delegateToTaskFunc)
        {
            _scheduler = (LinearScheduler<T>)Scheduler;
        }
    }

    public class AsyncGlyphScheduler<T, TDelegate, TParam> : GlyphSchedulerBase<T, TDelegate>
    {
        private readonly AsyncScheduler<T, TParam> _scheduler;

        public AsyncGlyphScheduler(Func<TDelegate, T> delegateToTaskFunc, Func<T, TParam, CancellationToken, Task> awaitableSelector)
            : base(new AsyncScheduler<T, TParam>(awaitableSelector), delegateToTaskFunc)
        {
            _scheduler = (AsyncScheduler<T, TParam>)Scheduler;
        }

        public Task RunScheduleAsync(TParam param) => _scheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _scheduler.RunScheduleAsync(param, cancellationToken);
    }

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

        public class TypedGroupDictionary : AssignableTypeDictionary<TypedGroup>
        {
        }

        public class TypedGroup : ObservableCollection<T>
        {
        }
    }
}