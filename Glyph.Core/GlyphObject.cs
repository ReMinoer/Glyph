using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Diese;
using Diese.Collections;
using Diese.Diagnostics;
using Niddle;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Exceptions;
using Glyph.Core.Resolvers;
using Glyph.Core.Scheduler;
using Glyph.Math;
using Glyph.Messaging;
using Glyph.Resolver;
using Taskete;

namespace Glyph.Core
{
    public class GlyphObject : GlyphComposite, IGlyphCompositeResolver, ILoadContent, IUpdate, IDraw, IBoxedComponent, IInterpreter
    {
        static public readonly WatchTree UpdateWatchTree = new WatchTree();

        private bool _initialized;
        private bool _contentLoaded;
        private readonly Dictionary<string, IGlyphComponent> _keyedComponents = new Dictionary<string, IGlyphComponent>();
        protected internal readonly GlyphCompositeDependencyResolver Resolver;

        public SchedulerHandler Schedulers { get; }
        public bool Visible { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public GlyphObject(GlyphResolveContext context)
        {
            Visible = true;

            var compositeResolver = new GlyphCompositeDependencyResolver(this, context);
            Resolver = compositeResolver;
            
            Resolver.Local.Registry.Add(GlyphDependency.OnType<TrackingRouter>().Using(Router.Local));

            Schedulers = new SchedulerHandler(compositeResolver.Global);
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
            {
                Schedulers.AssignComponent(glyphObject);
                glyphObject.Resolver.Parent = this;
            }
            else
                Schedulers.AssignComponent(item);

            if (_initialized)
                item.Initialize();

            if (_contentLoaded && item is ILoadContent loadingItem)
                Task.Run(async () => await loadingItem.LoadContent(Resolver.Resolve<IContentLibrary>())).Wait();
        }

        public IGlyphComponent GetComponent(string key) => key.StartsWith("$") ? throw new ArgumentException() : GetComponentBase(key);
        public bool SetComponent(string key, IGlyphComponent component) => key.StartsWith("$") ? throw new ArgumentException() : SetComponentBase(key, component);

        protected IGlyphComponent GetComponentProperty([CallerMemberName] string propertyName = null) => GetComponentBase('$' + propertyName);
        protected bool SetComponentProperty(IGlyphComponent component, [CallerMemberName] string propertyName = null) => SetComponentBase('$' + propertyName, component);

        private IGlyphComponent GetComponentBase(string key)
        {
            return _keyedComponents.TryGetValue(key, out IGlyphComponent component) ? component : null;
        }

        private bool SetComponentBase(string key, IGlyphComponent component)
        {
            IGlyphComponent currentComponent = GetComponentBase(key);
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

        public override sealed bool Remove(IGlyphComponent item)
        {
            if (!Contains(item) || !base.Remove(item))
                return false;

            _keyedComponents.Remove(x => x.Value == item);

            if (item is GlyphObject glyphObject)
                Schedulers.RemoveComponent(glyphObject);
            else
                Schedulers.RemoveComponent(item);

            return true;
        }

        public void RemoveAndDispose(IGlyphComponent item)
        {
            Remove(item);
            item.Dispose();
        }

        public override sealed void Clear()
        {
            base.Clear();

            _keyedComponents.Clear();
            Schedulers.ClearComponents();
        }

        public override sealed void Initialize()
        {
            if (Disposed)
                return;

            foreach (InitializeDelegate initialize in Schedulers.Initialize.Planning.ToArray())
            {
                if (Disposed)
                    return;

                initialize();
            }

            _initialized = true;
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            if (Disposed)
                return;
            
            await Task.WhenAll(Schedulers.LoadContent.Planning.Select(async x => await x(contentLibrary)).ToArray());
            _contentLoaded = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (Disposed || !Enabled)
                return;

            foreach (UpdateDelegate update in Schedulers.Update.Planning.ToArray())
            {
                if (Disposed || !Enabled)
                    return;

                using (UpdateWatchTree.Start($"{update.Target?.GetType().GetDisplayName()} -- {update.Method.Name}"))
                    update(elapsedTime);
            }
        }
        
        public void Draw(IDrawer drawer)
        {
            if (Disposed || !this.Displayed(drawer, drawer.Client))
                return;

            foreach (DrawDelegate draw in Schedulers.Draw.Planning.ToArray())
                draw(drawer);
        }
        
        IArea IBoxedComponent.Area => MathUtils.GetBoundingBox(Components.OfType<IBoxedComponent>().Select(x => x.Area));

        public class SchedulerHandler : GlyphSchedulerHandler
        {
            private readonly IDependencyResolver _resolver;
            public GlyphScheduler<IGlyphComponent, InitializeDelegate> Initialize { get; }
            public GlyphScheduler<ILoadContent, LoadContentDelegate> LoadContent { get; }
            public GlyphScheduler<IUpdate, UpdateDelegate> Update { get; }
            public GlyphScheduler<IDraw, DrawDelegate> Draw { get; }

            public SchedulerHandler(IDependencyResolver resolver)
            {
                _resolver = resolver;

                Initialize = AddScheduler<IGlyphComponent, InitializeDelegate>();
                LoadContent = AddScheduler<ILoadContent, LoadContentDelegate>();
                Update = AddScheduler<IUpdate, UpdateDelegate>();
                Draw = AddScheduler<IDraw, DrawDelegate>();
            }

            public GlyphScheduler<TInterface, TDelegate> AddScheduler<TInterface, TDelegate>()
                where TInterface : class, IGlyphComponent
            {
                var glyphScheduler = _resolver
                    .WithLink<IReadOnlyScheduler<Predicate<object>>, IReadOnlyScheduler<Predicate<object>>>(typeof(TInterface))
                    .Resolve<GlyphScheduler<TInterface, TDelegate>>();

                Schedulers.Add(glyphScheduler);
                return glyphScheduler;
            }
        }
    }
}