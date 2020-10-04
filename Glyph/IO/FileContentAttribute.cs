using System;

namespace Glyph.IO
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FileContentAttribute : Attribute
    {
        public Type ContentType { get; }

        public FileContentAttribute(Type contentType)
        {
            ContentType = contentType;
        }
    }
}