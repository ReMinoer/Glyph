using System;
using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    static class MessageHelper
    {
        static public IMessage BuildGeneric(Type messageTypeDefinition, object instance)
        {
            Type type = instance.GetType();
            Type messageType = messageTypeDefinition.MakeGenericType(type);
            return (IMessage)messageType.GetConstructor(new[] { type })?.Invoke(new[] { instance });
        }
    }
}