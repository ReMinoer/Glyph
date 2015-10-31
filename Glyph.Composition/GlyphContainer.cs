using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Composition;
using Diese.Injection;

namespace Glyph.Composition
{
    public abstract class GlyphContainer : GlyphContainer<IGlyphComponent>
    {
        protected GlyphContainer(int size)
            : base(size)
        {
        }
    }

    public abstract class GlyphContainer<TComponent> : Container<IGlyphComponent, IGlyphParent, TComponent>, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        private readonly IEnumerable<PropertyInfo> _injectableProperties;
        public virtual bool IsStatic { get; protected set; }

        IEnumerable<PropertyInfo> IGlyphComponent.InjectableProperties
        {
            get { return _injectableProperties; }
        }

        protected GlyphContainer(int size)
            : base(size)
        {
            _injectableProperties = GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(InjectableAttribute)).Any());
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            foreach (TComponent component in Components)
                component.Dispose();
        }
    }
}