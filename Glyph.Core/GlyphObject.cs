using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Diese.Collections;
using Diese.Diagnostics;
using Niddle;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Exceptions;
using Glyph.Composition.Utils;
using Glyph.Core.Resolvers;
using Glyph.Core.Scheduler;
using Glyph.Math;
using Glyph.Messaging;
using Glyph.Resolver;

namespace Glyph.Core
{
    public class GlyphObject : GlyphComposite, IGlyphCompositeResolver, ILoadContent, IDraw, IBoxedComponent, IInterpreter
    {
        static public readonly WatchTree UpdateWatchTree = new WatchTree();

        private bool _initialized;
        private bool _contentLoaded;
        private readonly Dictionary<string, IGlyphComponent> _keyedComponents = new Dictionary<string, IGlyphComponent>();
        protected internal readonly GlyphCompositeDependencyResolver Resolver;

        [Category(ComponentCategory.Automation)]
        public SchedulerHandler Schedulers { get; }

        [Category(ComponentCategory.Activation)]
        public bool Visible { get; set; }

        [Category(ComponentCategory.Automation)]
        public Predicate<IDrawer> DrawPredicate { get; set; }

        [Category(ComponentCategory.Automation)]
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public GlyphObject(GlyphResolveContext context)
        {
            Visible = true;

            var compositeResolver = new GlyphCompositeDependencyResolver(this, context);
            Resolver = compositeResolver;
            
            Resolver.Local.Registry.Add(GlyphDependency.OnType<TrackingRouter>().Using(Router.Local));

            Schedulers = new SchedulerHandler(context.GlobalResolver);
        }

        public T Add<T>()
            where T : IGlyphComponent
        {
            return Resolver.Add<T>();
        }

        public T Add<T>(Action<T> beforeAdding)
            where T : IGlyphComponent
        {
            return Resolver.Add(beforeAdding);
        }

        public IGlyphComponent Add(Type componentType)
        {
            return Resolver.Add(componentType) as IGlyphComponent;
        }

        // TODO : Handle injection on changing children & parents
        public override sealed void Add(IGlyphComponent item)
        {
            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();
            if (Components.AnyOfType(type) && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                glyphObject.Resolver.Parent = null;

            base.Add(item);

            if (glyphObject != null)
                glyphObject.Resolver.Parent = this;
            
            Schedulers.PlanComponent(item);

            if (_initialized)
                item.Initialize();

            if (_contentLoaded && item is ILoadContent loadingItem)
                Task.Run(async () => await loadingItem.LoadContent(Resolver.Resolve<IContentLibrary>())).Wait();
        }

        public IGlyphComponent GetKeyedComponent(string key)
        {
            return _keyedComponents.TryGetValue(key, out IGlyphComponent component) ? component : null;
        }

        public bool SetKeyedComponent(string key, IGlyphComponent component)
        {
            IGlyphComponent currentComponent = GetKeyedComponent(key);
            if (component == currentComponent)
                return false;

            if (currentComponent != null)
            {
                RemoveAndDispose(currentComponent);
                _keyedComponents.Remove(key);
            }

            if (component != null)
            {
                Add(component);
                _keyedComponents[key] = component;
            }

            return true;
        }

        protected bool SetPropertyComponent<T>(ref T component, T value, bool disposeOnRemove = false)
            where T : class, IGlyphComponent
        {
            if (component == value)
                return false;

            if (component != null)
            {
                Remove(component);
                if (disposeOnRemove)
                    component.Dispose();
            }

            component = value;

            if (component != null)
            {
                Add(component);
            }

            return true;
        }

        public override sealed bool Remove(IGlyphComponent item)
        {
            if (!Contains(item) || !base.Remove(item))
                return false;

            _keyedComponents.Remove(x => x.Value == item);

            Schedulers.UnplanComponent(item);

            return true;
        }

        public void RemoveAndDispose(IGlyphComponent item)
        {
            Remove(item);
            item.Dispose();
        }

        public override sealed void Clear()
        {
            foreach (IGlyphComponent component in Components)
                Schedulers.UnplanComponent(component);

            _keyedComponents.Clear();

            base.Clear();

        }

        public override sealed void Initialize()
        {
            if (IsDisposed)
                return;

            foreach (IInitializeTask task in Schedulers.Initialize.Schedule)
            {
                if (IsDisposed)
                    return;

                task.Initialize();
            }

            _initialized = true;
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            if (IsDisposed)
                return;

            await Schedulers.LoadContent.RunScheduleAsync(contentLibrary);
            _contentLoaded = true;
        }

        public void Draw(IDrawer drawer)
        {
            if (IsDisposed || !this.Displayed(drawer, drawer.Client))
                return;

            foreach (IDrawTask task in Schedulers.Draw.Schedule)
                task.Draw(drawer);
        }

        IArea IBoxedComponent.Area => MathUtils.GetBoundingBox(Components.OfType<IBoxedComponent>().Select(x => x.Area));
    }

    public class SchedulerHandler
    {
        public GlyphObjectScheduler<IInitializeTask, InitializeDelegate> Initialize { get; }
        public AsyncGlyphObjectScheduler<ILoadContentTask, LoadContentDelegate, IContentLibrary> LoadContent { get; }
        public GlyphObjectScheduler<IUpdateTask, UpdateDelegate> Update { get; }
        public GlyphObjectScheduler<IDrawTask, DrawDelegate> Draw { get; }

        public SchedulerHandler(IDependencyResolver resolver)
        {
            var initializeScheduler = new InitializeScheduler();
            resolver.Resolve<Action<InitializeScheduler>>().Invoke(initializeScheduler);

            var loadContentScheduler = new LoadContentScheduler();
            resolver.Resolve<Action<LoadContentScheduler>>().Invoke(loadContentScheduler);

            var updateScheduler = resolver.Resolve<UpdateScheduler>();

            var drawScheduler = new DrawScheduler();
            resolver.Resolve<Action<DrawScheduler>>().Invoke(drawScheduler);

            Initialize = new GlyphObjectScheduler<IInitializeTask, InitializeDelegate>(initializeScheduler);
            LoadContent = new AsyncGlyphObjectScheduler<ILoadContentTask, LoadContentDelegate, IContentLibrary>(loadContentScheduler);
            Update = new GlyphObjectScheduler<IUpdateTask, UpdateDelegate>(updateScheduler);
            Draw = new GlyphObjectScheduler<IDrawTask, DrawDelegate>(drawScheduler);
        }

        public void PlanComponent(IGlyphComponent component)
        {
            Initialize.Plan(component);
            if (component is ILoadContent loadContent)
                LoadContent.Plan(loadContent);
            if (component is IUpdate update)
                Update.Plan(update);
            if (component is IDraw draw)
                Draw.Plan(draw);
        }

        public void UnplanComponent(IGlyphComponent component)
        {
            Initialize.Unplan(component);
            if (component is ILoadContent loadContent)
                LoadContent.Unplan(loadContent);
            if (component is IUpdate update)
                Update.Unplan(update);
            if (component is IDraw draw)
                Draw.Unplan(draw);
        }
    }

    public class GlyphObjectScheduler<T, TDelegate> : GlyphObjectSchedulerBase<T, TDelegate>
    {
        private readonly GlyphScheduler<T, TDelegate> _glyphScheduler;
        public IEnumerable<T> Schedule => _glyphScheduler.Schedule;

        public GlyphObjectScheduler(GlyphScheduler<T, TDelegate> glyphScheduler)
            : base(glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;
        }
    }

    public class AsyncGlyphObjectScheduler<T, TDelegate, TParam> : GlyphObjectSchedulerBase<T, TDelegate>
    {
        private readonly AsyncGlyphScheduler<T, TDelegate, TParam> _glyphScheduler;

        public AsyncGlyphObjectScheduler(AsyncGlyphScheduler<T, TDelegate, TParam> glyphScheduler)
            : base(glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;
        }

        public Task RunScheduleAsync(TParam param) => _glyphScheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _glyphScheduler.RunScheduleAsync(param, cancellationToken);
    }

    public abstract class GlyphObjectSchedulerBase<T, TDelegate>
        : IGlyphScheduler<T, TDelegate, GlyphObjectSchedulerBase<T, TDelegate>.TaskController, GlyphObjectSchedulerBase<T, TDelegate>.Controller>
    {
        private readonly GlyphSchedulerBase<T, TDelegate> _glyphScheduler;
        private IGlyphScheduler<T, TDelegate> GlyphScheduler => _glyphScheduler;

        private readonly PriorityGroupDictionary _priorityGroups = new PriorityGroupDictionary();

        public GlyphObjectSchedulerBase(GlyphSchedulerBase<T, TDelegate> glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;

            _priorityGroups.Add(Priority.High, new PriorityGroup());
            _priorityGroups.Add(Priority.Normal, new PriorityGroup());
            _priorityGroups.Add(Priority.Low, new PriorityGroup());
            
            glyphScheduler.Plan(_priorityGroups[Priority.High]).Before(_priorityGroups[Priority.Normal]);
            glyphScheduler.Plan(_priorityGroups[Priority.Normal]).Before(_priorityGroups[Priority.Low]);
        }

        public TaskController Plan(T task)
        {
            GlyphScheduler.Plan(task);
            InitPriority(task);

            return new TaskController(this, _glyphScheduler, new[] { task });
        }

        public TaskController Plan(IEnumerable<T> tasks)
        {
            GlyphScheduler.Plan(tasks);
            foreach (T task in tasks)
                InitPriority(task);

            return new TaskController(this, _glyphScheduler, tasks);
        }

        public TaskController Plan(TDelegate taskDelegate)
        {
            GlyphScheduler.Plan(taskDelegate);

            T task = _glyphScheduler.GetDelegateTask(taskDelegate);
            InitPriority(task);

            return new TaskController(this, _glyphScheduler, new[] { task });
        }

        void IGlyphScheduler<T>.Plan(T task) => Plan(task);
        void IGlyphScheduler<T>.Plan(IEnumerable<T> tasks) => Plan(tasks);
        void IGlyphScheduler<T, TDelegate>.Plan(TDelegate taskDelegate) => Plan(taskDelegate);

        public Controller Plan<TTasks>() => Plan(typeof(TTasks));
        public Controller Plan(Type taskType) => new Controller(this, _glyphScheduler, _glyphScheduler.GetOrAddTypedGroup(taskType));

        public void Unplan(T task)
        {
            CleanPriority(task);
            GlyphScheduler.Unplan(task);
        }

        public void Unplan(TDelegate taskDelegate) => Unplan(_glyphScheduler.GetDelegateTask(taskDelegate));

        private void InitPriority(T task)
        {
            if (!_priorityGroups[Priority.Normal].Contains(task))
                _priorityGroups[Priority.Normal].Add(task);
        }

        private void CleanPriority(T task)
        {
            foreach (PriorityGroup priorityGroup in _priorityGroups.Values)
                priorityGroup.Remove(task);
        }

        public class Controller : ControllerBase<Controller>
        {
            public Controller(GlyphObjectSchedulerBase<T, TDelegate> glyphObjectScheduler, GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> tasks)
                : base(glyphObjectScheduler, glyphScheduler, tasks)
            {
            }

            protected override Controller ReturnedController => this;
        }

        public class TaskController : ControllerBase<TaskController>
        {
            private Priority _priority;

            public TaskController(GlyphObjectSchedulerBase<T, TDelegate> glyphObjectScheduler, GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> controlledTasks)
                : base(glyphObjectScheduler, glyphScheduler, controlledTasks)
            {
                _priority = Priority.Normal;
            }

            protected override TaskController ReturnedController => this;

            public TaskController AtStart()
            {
                SwitchPriorityGroup(Priority.High);
                return ReturnedController;
            }

            public TaskController AtEnd()
            {
                SwitchPriorityGroup(Priority.Low);
                return ReturnedController;
            }

            private void SwitchPriorityGroup(Priority priority)
            {
                foreach (T controlledTask in ControlledTasks)
                    GlyphObjectScheduler._priorityGroups[_priority].Remove(controlledTask);

                _priority = priority;

                foreach (T controlledTask in ControlledTasks)
                    GlyphObjectScheduler._priorityGroups[_priority].Add(controlledTask);
            }
        }

        public abstract class ControllerBase<TController> : GlyphSchedulerBase<T, TDelegate>.ControllerBase<TController>
        {
            protected readonly GlyphObjectSchedulerBase<T, TDelegate> GlyphObjectScheduler;

            public ControllerBase(GlyphObjectSchedulerBase<T, TDelegate> glyphObjectScheduler, GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> tasks)
                : base(glyphScheduler, tasks)
            {
                GlyphObjectScheduler = glyphObjectScheduler;
            }
        }

        private enum Priority
        {
            High,
            Normal,
            Low
        }

        private class PriorityGroupDictionary : Dictionary<Priority, PriorityGroup>
        {
        }

        private class PriorityGroup : ObservableCollection<T>
        {
        }
    }
}