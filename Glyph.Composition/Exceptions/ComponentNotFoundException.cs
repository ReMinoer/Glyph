using System;
using Niddle.Exceptions;

namespace Glyph.Composition.Exceptions
{
    public class ComponentNotFoundException : NotRegisterException
    {
        new private const string Message = "Component of type {0} not found !";

        public ComponentNotFoundException(Type type)
            : base(string.Format(Message, type.Name))
        {
        }
    }
}