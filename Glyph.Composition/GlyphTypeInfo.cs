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
        public IEnumerable<InjectablePropertyInfo> InjectableProperties => _readOnlyInjectableProperties;

        internal GlyphTypeInfo(Type type)
        {
            var injectableProperties = new List<InjectablePropertyInfo>();
            _readOnlyInjectableProperties = injectableProperties.AsReadOnly();

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<InjectableAttributeBase>();
                if (attribute == null)
                    continue;

                injectableProperties.Add(new InjectablePropertyInfo(propertyInfo.PropertyType, propertyInfo, attribute as IGlyphInjectableAttribute));
            }
        }
    }
}