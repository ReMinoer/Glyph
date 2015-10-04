using System;
using System.Linq;
using System.Reflection;
using Diese.Injection;
using Glyph.Composition.Exceptions;
using Glyph.Composition.Scheduler;

namespace Glyph.Composition
{
    // TASK : Initialize when add a component after parent Initialize
    public abstract class GlyphSchedulableBase : GlyphComposite
    {
        protected readonly GlyphCompositeInjector Injector;
        protected abstract IGlyphSchedulerAssigner SchedulerAssigner { get; }

        protected GlyphSchedulableBase(IDependencyInjector injector)
        {
            var compositeInjector = injector.Resolve<GlyphCompositeInjector>();
            compositeInjector.CompositeContext = this;

            Injector = compositeInjector;
        }

        public T Add<T>()
        {
            return Injector.Add<T>();
        }

        public IGlyphComponent Add(Type componentType)
        {
            return Injector.Add(componentType) as IGlyphComponent;
        }

        public override sealed void Add(IGlyphComponent item)
        {
            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();

            if (GetComponent(type) != null && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            base.Add(item);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                SchedulerAssigner.AssignComponent(glyphObject);
            else
                SchedulerAssigner.AssignComponent(item);
        }

        public override sealed void Remove(IGlyphComponent item)
        {
            if (!Contains(item))
                throw new ArgumentException("Component provided is not contained by this entity !");

            base.Remove(item);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                SchedulerAssigner.RemoveComponent(glyphObject);
            else
                SchedulerAssigner.RemoveComponent(item);
        }

        public override sealed void Clear()
        {
            SchedulerAssigner.ClearComponents();

            base.Clear();
        }
    }
}