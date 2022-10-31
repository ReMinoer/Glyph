using System;
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
using CategoryAttribute = System.ComponentModel.CategoryAttribute;
using Simulacra.Injection;

namespace Glyph.Core
{
    public class GlyphObject : GlyphComposite, IGlyphCompositeResolver, ILoadContent, IBoxedComponent, IInterpreter
    {
        private bool _initialized;
        private bool _contentLoaded;

        public GlyphCompositeDependencyResolver Resolver { get; }
        IDependencyResolver IGlyphCompositeResolver.DependencyResolver => Resolver;

        [Category(ComponentCategory.Automation)]
        public ComponentSchedulerHandler Schedulers { get; }

        [Category(ComponentCategory.Automation)]
        public Predicate<IDrawer> DrawPredicate { get; set; }

        [Category(ComponentCategory.Automation)]
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public virtual IArea Area => MathUtils.GetBoundingBox(Components.OfType<IBoxedComponent>().Select(x => x.Area));

        public GlyphObject(GlyphResolveContext context)
        {
            var compositeResolver = new GlyphCompositeDependencyResolver(this, context);
            Resolver = compositeResolver;
            
            Resolver.Local.Registry.Add(GlyphDependency.OnType<TrackingRouter>().Using(Router.Local));

            Schedulers = new ComponentSchedulerHandler(context.GlobalResolver, this);
        }

        public override IGlyphComponent this[int index]
        {
            get => base[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
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

        public override sealed void Add(IGlyphComponent item)
        {
            Insert(Components.Count, item);
        }

        // TODO : Handle injection on changing children & parents
        public override void Insert(int index, IGlyphComponent item)
        {
            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();
            if (Components.AnyOfType(type) && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                glyphObject.Resolver.Parent = null;
            
            base.Insert(index, item);

            if (glyphObject != null)
                glyphObject.Resolver.Parent = this;
            
            Schedulers.PlanComponent(item);

            if (_initialized)
                item.Initialize();

            if (_contentLoaded && item is ILoadContent loadingItem)
                loadingItem.LoadContent(Resolver.Resolve<IContentLibrary>());
        }

        public override sealed bool Remove(IGlyphComponent item)
        {
            if (!Contains(item))
                return false;
            
            RemoveItem(item);
            return true;
        }

        public override void RemoveAt(int index)
        {
            if (index < 0 || index > Components.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Insert index {index} out of range.");
            
            RemoveItem(Components[index]);
        }

        private void RemoveItem(IGlyphComponent item)
        {
            Schedulers.UnplanComponent(item);

            base.Remove(item);
        }

        public override sealed void Clear()
        {
            foreach (IGlyphComponent component in Components)
                Schedulers.UnplanComponent(component);

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

        public void LoadContent(IContentLibrary contentLibrary)
        {
            if (IsDisposed)
                return;

            Schedulers.LoadContent.RunSchedule(contentLibrary);
            _contentLoaded = true;
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            if (IsDisposed)
                return;

            await Schedulers.LoadContent.RunScheduleAsync(contentLibrary);
            _contentLoaded = true;
        }
    }
}