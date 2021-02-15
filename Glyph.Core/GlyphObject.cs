using System;
using System.Collections.Generic;
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
using Taskete.Rules;

namespace Glyph.Core
{
    public class GlyphObject : GlyphComposite, IGlyphCompositeResolver, ILoadContent, IUpdate, IDraw, IBoxedComponent, IInterpreter
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

        public void Update(ElapsedTime elapsedTime)
        {
            if (IsDisposed || !Enabled)
                return;

            foreach (IUpdateTask task in Schedulers.Update.Schedule)
            {
                if (IsDisposed || !Enabled)
                    return;

                //using (UpdateWatchTree.Start($"{task.Target?.GetType().GetDisplayName()} -- {update.Method.Name}"))
                    task.Update(elapsedTime);
            }
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
            var initializeScheduler = new GlyphScheduler<IInitializeTask>();
            var loadContentScheduler = new AsyncGlyphScheduler<ILoadContentTask, IContentLibrary>((x, contentLibrary, _) => x.LoadContent(contentLibrary));
            var updateScheduler = new GlyphScheduler<IUpdateTask>();
            var drawScheduler = new GlyphScheduler<IDrawTask>();

            resolver.Resolve<Action<GlyphScheduler<IInitializeTask>>>().Invoke(initializeScheduler);
            resolver.Resolve<Action<AsyncGlyphScheduler<ILoadContentTask, IContentLibrary>>>().Invoke(loadContentScheduler);
            resolver.Resolve<Action<GlyphScheduler<IUpdateTask>>>().Invoke(updateScheduler);
            resolver.Resolve<Action<GlyphScheduler<IDrawTask>>>().Invoke(drawScheduler);

            Initialize = new GlyphObjectScheduler<IInitializeTask, InitializeDelegate>(initializeScheduler, x => new InitializeTask(x));
            LoadContent = new AsyncGlyphObjectScheduler<ILoadContentTask, LoadContentDelegate, IContentLibrary>(loadContentScheduler, x => new LoadContentTask(x));
            Update = new GlyphObjectScheduler<IUpdateTask, UpdateDelegate>(updateScheduler, x => new UpdateTask(x));
            Draw = new GlyphObjectScheduler<IDrawTask, DrawDelegate>(drawScheduler, x => new DrawTask(x));
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

        public class InitializeTask : IInitializeTask
        {
            private readonly InitializeDelegate _taskDelegate;
            public InitializeTask(InitializeDelegate taskDelegate) => _taskDelegate = taskDelegate;
            public void Initialize() => _taskDelegate();
        }

        public class LoadContentTask : ILoadContentTask
        {
            private readonly LoadContentDelegate _taskDelegate;
            public LoadContentTask(LoadContentDelegate taskDelegate) => _taskDelegate = taskDelegate;
            public Task LoadContent(IContentLibrary contentLibrary) => _taskDelegate(contentLibrary);
        }

        public class UpdateTask : IUpdateTask
        {
            private readonly UpdateDelegate _taskDelegate;
            public UpdateTask(UpdateDelegate taskDelegate) => _taskDelegate = taskDelegate;
            public void Update(ElapsedTime elapsedTime) => _taskDelegate(elapsedTime);
        }

        public class DrawTask : IDrawTask
        {
            private readonly DrawDelegate _taskDelegate;
            public DrawTask(DrawDelegate taskDelegate) => _taskDelegate = taskDelegate;
            public void Draw(IDrawer drawer) => _taskDelegate(drawer);
        }
    }

    public class GlyphObjectScheduler<T, TDelegate> : GlyphObjectSchedulerBase<T, TDelegate>
    {
        private readonly GlyphScheduler<T> _glyphScheduler;
        public IEnumerable<T> Schedule => _glyphScheduler.Schedule;

        public GlyphObjectScheduler(GlyphScheduler<T> glyphScheduler, Func<TDelegate, T> delegateToTaskFunc)
            : base(glyphScheduler, delegateToTaskFunc)
        {
            _glyphScheduler = glyphScheduler;
        }
    }

    public class AsyncGlyphObjectScheduler<T, TDelegate, TParam> : GlyphObjectSchedulerBase<T, TDelegate>
    {
        private readonly AsyncGlyphScheduler<T, TParam> _glyphScheduler;

        public AsyncGlyphObjectScheduler(AsyncGlyphScheduler<T, TParam> glyphScheduler, Func<TDelegate, T> delegateToTaskFunc)
            : base(glyphScheduler, delegateToTaskFunc)
        {
            _glyphScheduler = glyphScheduler;
        }

        public Task RunScheduleAsync(TParam param) => _glyphScheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _glyphScheduler.RunScheduleAsync(param, cancellationToken);
    }

    public abstract class GlyphObjectSchedulerBase<T, TDelegate>
        : IGlyphScheduler<T, GlyphObjectSchedulerBase<T, TDelegate>.ItemController, GlyphObjectSchedulerBase<T, TDelegate>.TypeController>
    {
        private GlyphSchedulerBase<T> _glyphScheduler;

        private readonly Dictionary<TDelegate, T> _delegateDictionary = new Dictionary<TDelegate, T>();
        private readonly Func<TDelegate, T> _delegateToTaskFunc;

        public GlyphObjectSchedulerBase(GlyphSchedulerBase<T> glyphScheduler, Func<TDelegate, T> delegateToTaskFunc)
        {
            _glyphScheduler = glyphScheduler;
            _delegateToTaskFunc = delegateToTaskFunc;
        }

        public ItemController Plan(TDelegate taskDelegate) => Plan(GetTask(taskDelegate));
        public ItemController Plan(T task)
        {
            _glyphScheduler.Plan(task);
            return new ItemController(this, _glyphScheduler, task);
        }
        void IGlyphScheduler<T>.Plan(T task)
        {
            _glyphScheduler.Plan(task);
        }

        public TypeController Plan<TTasks>() => Plan(typeof(TTasks));
        public TypeController Plan(Type taskType) => new TypeController(this, _glyphScheduler, taskType);

        public void Unplan(T task) => _glyphScheduler.Unplan(task);

        private T GetTask(TDelegate taskDelegate)
        {
            if (_delegateDictionary.TryGetValue(taskDelegate, out T task))
                return task;

            task = _delegateToTaskFunc(taskDelegate);
            _delegateDictionary.Add(taskDelegate, task);
            return task;
        }

        public class ItemController : GlyphSchedulerBase<T>.ItemController<ItemController>
        {
            private readonly GlyphObjectSchedulerBase<T, TDelegate> _glyphObjectScheduler;

            public ItemController(GlyphObjectSchedulerBase<T, TDelegate> glyphObjectScheduler, GlyphSchedulerBase<T> glyphScheduler, params T[] controlledTasks)
                : base(glyphScheduler, controlledTasks)
            {
                _glyphObjectScheduler = glyphObjectScheduler;
            }

            protected override ItemController Controller => this;

            public ItemController Before(TDelegate taskDelegate, float? weight = null)
            {
                T task = _glyphObjectScheduler.GetTask(taskDelegate);
                AddTask(task);
                AddRule(DependencyRule<T>.New(ControlledTasks, new[] { task }), weight);
                return this;
            }

            public ItemController After(TDelegate taskDelegate, float? weight = null)
            {
                T task = _glyphObjectScheduler.GetTask(taskDelegate);
                AddTask(task);
                AddRule(DependencyRule<T>.New(new[] { task }, ControlledTasks), weight);
                return this;
            }
        }

        public class TypeController : GlyphSchedulerBase<T>.TypeController<TypeController>
        {
            private readonly GlyphObjectSchedulerBase<T, TDelegate> _glyphObjectScheduler;

            public TypeController(GlyphObjectSchedulerBase<T, TDelegate> glyphObjectScheduler, GlyphSchedulerBase<T> glyphScheduler, Type taskType)
                : base(glyphScheduler, taskType)
            {
                _glyphObjectScheduler = glyphObjectScheduler;
            }

            protected override TypeController Controller => this;

            public TypeController Before(TDelegate taskDelegate, float? weight = null)
            {
                T task = _glyphObjectScheduler.GetTask(taskDelegate);
                AddTask(task);
                AddRule(DependencyRule<T>.New(ControlledTypedGroup, new[] { task }), weight);
                return this;
            }

            public TypeController After(TDelegate taskDelegate, float? weight = null)
            {
                T task = _glyphObjectScheduler.GetTask(taskDelegate);
                AddTask(task);
                AddRule(DependencyRule<T>.New(new[] { task }, ControlledTypedGroup), weight);
                return this;
            }
        }
    }
}