using System;
using Diese.Injection;
using Glyph.Composition.Exceptions;

namespace Glyph.Composition
{
    public class GlyphCompositeInjector : RegistryInjector
    {
        internal GlyphComposite CompositeContext { private get; set; }

        public IDependencyInjector Base
        {
            get { return Resolve<IDependencyInjector>(); }
        }

        public GlyphCompositeInjector(IDependencyRegistry registry)
            : base(registry)
        {
        }

        public override object Resolve(Type type, object serviceKey = null)
        {
            if (serviceKey == null && typeof(IGlyphComponent).IsAssignableFrom(type))
            {
                IGlyphComponent component = CompositeContext.GetComponent(type);
                if (component == null)
                    throw new ComponentNotFoundException(type);

                return component;
            }
            
            return base.Resolve(type, serviceKey);
        }

        internal T Add<T>()
        {
            return (T)Add(typeof(T));
        }

        internal object Add(Type type)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(type))
                throw new InvalidCastException(string.Format("Type must implements {0} !", typeof(IGlyphComponent)));

            var component = (IGlyphComponent)base.Resolve(type);
            CompositeContext.Add(component);

            return component;
        }
    }
}