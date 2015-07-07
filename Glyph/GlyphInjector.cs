using System;
using Diese.Injection;

namespace Glyph
{
    public class GlyphInjector : IDependencyInjector
    {
        private readonly IGlyphParent _parent;
        private readonly IDependencyRegistry _registry;

        public GlyphInjector(IGlyphParent parent, IDependencyRegistry registry)
        {
            _parent = parent;
            _registry = registry;
        }

        public T Resolve<T>(object serviceKey = null)
        {
            return (T)Resolve(typeof(T), serviceKey);
        }

        public object Resolve(Type type, object serviceKey = null)
        {
            IGlyphComponent component = _parent.GetComponent(type);
            if (component != null)
                return component;

            object dependency = _registry[type, serviceKey].Get(this);

            if (!_parent.IsReadOnly && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                component = (IGlyphComponent)dependency;
                if (component.Parent != null)
                    _parent.Link(component);
            }

            return dependency;
        }
    }
}