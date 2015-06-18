using System;

namespace Glyph
{
    public abstract class DependencyAttribute : Attribute
    {
        public Type ComponentType { get; private set; }

        protected DependencyAttribute(Type componentType)
        {
            if (!typeof(IGlyphComponent).IsAssignableFrom(componentType))
                throw new ArgumentException("Dependency must implements " + typeof(IGlyphComponent).Name + " !");

            ComponentType = componentType;
        }
    }
}