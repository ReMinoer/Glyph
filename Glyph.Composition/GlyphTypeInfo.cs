using System;
using System.Collections.Generic;
using System.Reflection;
using Diese.Injection;
using Glyph.Injection;

namespace Glyph.Composition
{
    public class GlyphTypeInfo
    {
        private readonly IReadOnlyCollection<InjectablePropertyInfo> _readOnlyInjectableProperties;

        public IEnumerable<InjectablePropertyInfo> InjectableProperties
        {
            get { return _readOnlyInjectableProperties; }
        }

        internal GlyphTypeInfo(Type type)
        {
            var injectableProperties = new List<InjectablePropertyInfo>();
            _readOnlyInjectableProperties = injectableProperties.AsReadOnly();

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<InjectableAttribute>();
                if (attribute == null)
                    continue;

                var glyphAttribute = attribute as GlyphInjectableAttribute;
                injectableProperties.Add(new InjectablePropertyInfo(propertyInfo, glyphAttribute != null ? glyphAttribute.Targets : GlyphInjectableTargets.All));
            }
        }
    }
}