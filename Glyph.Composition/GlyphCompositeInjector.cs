using System;
using Diese.Injection;
using Glyph.Composition.Exceptions;

namespace Glyph.Composition
{
    public class GlyphCompositeInjector : IDependencyInjector
    {
        private readonly IDependencyInjector _dependencyInjector;
        internal GlyphComposite CompositeContext { private get; set; }

        public GlyphCompositeInjector(IDependencyInjector dependencyInjector)
        {
            _dependencyInjector = dependencyInjector;
        }

        public T Resolve<T>(object serviceKey = null)
        {
            return (T)Resolve(typeof(T), serviceKey);
        }

        public object Resolve(Type type, object serviceKey = null)
        {
            if (serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                IGlyphComponent component = CompositeContext.GetComponent(type);
                if (component == null)
                    throw new ComponentNotFoundException(type);

                return component;
            }
            
            return _dependencyInjector.Resolve(type, serviceKey);
        }

        public T Add<T>()
        {
            return (T)Add(typeof(T));
        }

        public object Add(Type type)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(type))
                throw new InvalidCastException(string.Format("Type must implements {0} !", typeof(IGlyphComponent)));

            var component = (IGlyphComponent)_dependencyInjector.Resolve(type);
            CompositeContext.Add(component);

            return component;
        }
    }
}