using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Composition;
using Diese.Injection;

namespace Glyph.Composition
{
    public class GlyphComponent : Component<IGlyphComponent, IGlyphParent>, IGlyphComponent
    {
        private readonly IEnumerable<PropertyInfo> _injectableProperties;

        IEnumerable<PropertyInfo> IGlyphComponent.InjectableProperties
        {
            get { return _injectableProperties; }
        }

        public GlyphComponent()
        {
            _injectableProperties = GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(InjectableAttribute)).Any());
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}