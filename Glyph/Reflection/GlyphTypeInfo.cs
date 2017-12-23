using System;
using System.Collections.Generic;
using System.Reflection;
using Diese.Injection;
using Glyph.Injection;

namespace Glyph.Reflection
{
    public class GlyphTypeInfo
    {
        public Type Type { get; }
        public IEnumerable<InjectablePropertyInfo> InjectableProperties { get; }

        internal GlyphTypeInfo(Type type)
        {
            Type = type;
            InjectableProperties = GetInjectableProperties(type);
        }

        private IEnumerable<InjectablePropertyInfo> GetInjectableProperties(Type type)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var attribute = propertyInfo.GetCustomAttribute<InjectableAttributeBase>();
                if (attribute == null)
                    continue;

                yield return new InjectablePropertyInfo(propertyInfo.PropertyType, propertyInfo, attribute as IGlyphInjectableAttribute);
            }
        }
    }
}