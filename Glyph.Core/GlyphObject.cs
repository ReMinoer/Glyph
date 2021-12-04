using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Diese.Collections;
using Niddle;
using Glyph.Composition;
using Glyph.Composition.Exceptions;
using Glyph.Composition.Utils;
using Glyph.Core.Resolvers;
using Glyph.Core.Schedulers;
using Glyph.Math;
using Glyph.Messaging;
using Glyph.Resolver;
using Glyph.Scheduling;

namespace Glyph.Core
{
    public class GlyphObject : GlyphComposite, IGlyphCompositeResolver, ILoadContent, IBoxedComponent, IInterpreter
    {
        private bool _initialized;
        private bool _contentLoaded;
        private readonly Dictionary<object, IGlyphComponent> _keyedComponents = new Dictionary<object, IGlyphComponent>();
        public GlyphCompositeDependencyResolver Resolver { get; }

        [Category(ComponentCategory.Automation)]
        public ComponentSchedulerHandler Schedulers { get; }

        [Category(ComponentCategory.Activation)]
        public bool Visible { get; set; }

        [Category(ComponentCategory.Automation)]
        public Predicate<IDrawer> DrawPredicate { get; set; }

        [Category(ComponentCategory.Automation)]
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public virtual IArea Area => MathUtils.GetBoundingBox(Components.OfType<IBoxedComponent>().Select(x => x.Area));

        public GlyphObject(GlyphResolveContext context)
        {
            Visible = true;

            var compositeResolver = new GlyphCompositeDependencyResolver(this, context);
            Resolver = compositeResolver;
            
            Resolver.Local.Registry.Add(GlyphDependency.OnType<TrackingRouter>().Using(Router.Local));

            Schedulers = new ComponentSchedulerHandler(context.GlobalResolver);
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

        public T GetKeyedComponent<T>(object key)
            where T : class, IGlyphComponent
        {
            return _keyedComponents.TryGetValue(key, out IGlyphComponent component) ? (T)component : default(T);
        }

        public bool SetKeyedComponent(object key, IGlyphComponent component)
        {
            IGlyphComponent currentComponent = GetKeyedComponent<IGlyphComponent>(key);
            if (component == currentComponent)
                return false;

            if (currentComponent != null)
            {
                RemoveAndDispose(currentComponent);
                _keyedComponents.Remove(key);
            }

            if (component != null)
            {
                if (!Contains(component))
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

        public void ClearAndDisposeComponents()
        {
            IGlyphComponent[] components = Components.ToArray();
            Clear();

            foreach (IGlyphComponent component in components)
                component.Dispose();
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
    }
}