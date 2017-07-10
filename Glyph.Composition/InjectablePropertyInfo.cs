using System;
using System.Reflection;
using Glyph.Injection;

namespace Glyph.Composition
{
    public class InjectablePropertyInfo
    {
        public Type Type { get; }
        public PropertyInfo PropertyInfo { get; }
        public IGlyphInjectableAttribute Attribute { get; }

        public InjectablePropertyInfo(Type type, PropertyInfo propertyInfo, IGlyphInjectableAttribute attribute)
        {
            Type = type;
            PropertyInfo = propertyInfo;
            Attribute = attribute;
        }
    }
}