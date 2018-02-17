using System;

namespace Glyph.Export
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyContainsAttribute : Attribute
    {
        public Type Type { get; }

        public AssemblyContainsAttribute(Type type)
        {
            Type = type;
        }
    }
}