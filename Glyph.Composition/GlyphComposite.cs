using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Composition;
using Diese.Injection;

namespace Glyph.Composition
{
    public abstract class GlyphComposite : GlyphComposite<IGlyphComponent>
    {
    }

    public abstract class GlyphComposite<TComponent> : Composite<IGlyphComponent, IGlyphParent, TComponent>, IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        private readonly IEnumerable<PropertyInfo> _injectableProperties;
        public virtual bool IsStatic { get; protected set; }

        IEnumerable<PropertyInfo> IGlyphComponent.InjectableProperties
        {
            get { return _injectableProperties; }
        }

        protected GlyphComposite()
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

            Clear();
        }
    }
}