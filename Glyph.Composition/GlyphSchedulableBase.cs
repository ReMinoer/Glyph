using System;
using System.Linq;
using System.Reflection;
using Diese.Injection;
using Glyph.Composition.Exceptions;
using Glyph.Composition.Injection;
using Glyph.Composition.Messaging;
using Glyph.Composition.Scheduler;

namespace Glyph.Composition
{
    public abstract class GlyphSchedulableBase : GlyphComposite
    {
        protected internal readonly GlyphCompositeInjector Injector;
        protected abstract IGlyphSchedulerAssigner SchedulerAssigner { get; }

        protected GlyphSchedulableBase(IDependencyInjector injector)
        {
            var compositeInjector = new GlyphCompositeInjector(this, injector.Resolve<IDependencyRegistry>(), injector.Resolve<IDependencyRegistry>(InjectionScope.Local));

            Injector = compositeInjector;

            foreach (Type type in GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IInterpreter<>)))
                Add(typeof(Receiver<>).MakeGenericType(type.GetGenericArguments()));
        }

        public T Add<T>()
        {
            return Injector.Add<T>();
        }

        public IGlyphComponent Add(Type componentType)
        {
            return Injector.Add(componentType) as IGlyphComponent;
        }

        public override void Add(IGlyphComponent item)
        {
            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();
            if (GetComponent(type) != null && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            base.Add(item);
            InjectToOtherComponents(item);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
            {
                SchedulerAssigner.AssignComponent(glyphObject);
                glyphObject.Injector.Parent = this;
            }
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
            base.Clear();

            SchedulerAssigner.ClearComponents();
        }

        public void SendMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            var router = Injector.Resolve<IRouter<TMessage>>();
            router.Send(message);
        }

        private void InjectToOtherComponents(IGlyphComponent item)
        {
            foreach (IGlyphComponent component in this)
                foreach (PropertyInfo property in component.InjectableProperties)
                    if (property.GetValue(component) == null && item.GetType().IsInstanceOfType(property.PropertyType))
                        property.SetValue(component, item);
        }
    }
}