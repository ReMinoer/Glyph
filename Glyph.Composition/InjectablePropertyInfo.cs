using System.Reflection;
using Glyph.Composition.Injection;

namespace Glyph.Composition
{
    public class InjectablePropertyInfo
    {
        public PropertyInfo PropertyInfo { get; private set; }
        public GlyphInjectableTargets InjectableTargets { get; private set; }

        public InjectablePropertyInfo(PropertyInfo propertyInfo, GlyphInjectableTargets injectableTargets)
        {
            PropertyInfo = propertyInfo;
            InjectableTargets = injectableTargets;
        }
    }
}