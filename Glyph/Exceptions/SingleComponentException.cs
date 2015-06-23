using System;

namespace Glyph.Exceptions
{
    public class SingleComponentException : GlyphException
    {
        private const string Message = "You can't have two components of type {0} on the same parent !";

        public SingleComponentException(Type type)
            : base(Message, type.Name)
        {
        }
    }
}