using System;
using Glyph.Exceptions;

namespace Glyph.Composition.Exceptions
{
    public class ComponentNotFoundException : GlyphException
    {
        new private const string Message = "Component of type {0} not found !";

        public ComponentNotFoundException(Type type)
            : base(Message, type.Name)
        {
        }
    }
}