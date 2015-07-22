using System;
using Glyph.Exceptions;

namespace Glyph.Composition.Exceptions
{
    public class SingleComponentException : GlyphException
    {
        new private const string Message = "You can't have two components of type {0} on the same parent !";

        public SingleComponentException(Type type)
            : base(Message, type.Name)
        {
        }
    }
}