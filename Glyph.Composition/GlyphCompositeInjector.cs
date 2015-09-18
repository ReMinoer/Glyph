using System;
using Diese.Injection;

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
                return CompositeContext.GetComponent(type) ?? _dependencyInjector.Resolve(type);
            
            return _dependencyInjector.Resolve(type, serviceKey);
        }
    }
}