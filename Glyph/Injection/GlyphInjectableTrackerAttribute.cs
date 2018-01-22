using System;
using System.Linq;
using System.Reflection;
using Diese.Collections;
using Diese.Injection;
using Diese.Injection.Base;

namespace Glyph.Injection
{
    public class GlyphInjectableTrackerAttribute : InjectableManyAttributeBase, IGlyphInjectableAttribute
    {
        public GlyphInjectableTargets Targets { get; }

        public GlyphInjectableTrackerAttribute()
        {
            Targets = GlyphInjectableTargets.All;
        }

        public GlyphInjectableTrackerAttribute(GlyphInjectableTargets targets)
        {
            Targets = targets;
        }

        public override Type GetInjectedType(Type memberType)
        {
            return memberType.GenericTypeArguments[0];
        }

        public override sealed void Inject(PropertyInfo propertyInfo, object obj, object value)
        {
            Type propertyType = propertyInfo.PropertyType;
            Type trackerType = TypePredicate(propertyType) ? propertyType : propertyType.GetInterfaces().First(TypePredicate);
            trackerType.GetMethod("Register").Invoke(propertyInfo.GetValue(obj), new[] { value });
        }

        public override sealed void Inject(FieldInfo fieldInfo, object obj, object value)
        {
            Type fieldType = fieldInfo.FieldType;
            Type trackerType = TypePredicate(fieldType) ? fieldType : fieldType.GetInterfaces().First(TypePredicate);
            trackerType.GetMethod("Register").Invoke(fieldInfo.GetValue(obj), new[] { value });
        }

        static private bool TypePredicate(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ITracker<>);
        }
    }
}