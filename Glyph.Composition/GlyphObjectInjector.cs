using System;
using Diese.Injection;

namespace Glyph.Composition
{
    public class GlyphObjectInjector : IDependencyInjector
    {
        private readonly IDependencyInjector _dependencyInjector;
        internal GlyphObject GlyphObject { private get; set; }

        public GlyphObjectInjector(IDependencyInjector dependencyInjector)
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
                return GlyphObject.GetComponent(type);
            
            return _dependencyInjector.Resolve(type, serviceKey);
        }
    }
}