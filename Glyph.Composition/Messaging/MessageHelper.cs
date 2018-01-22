using System;
using System.Reflection;
using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    static class MessageHelper
    {
        static public IMessage BuildGeneric(Type messageTypeDefinition, Type messageTypeArgument, Func<Type, ConstructorInfo> constructorProvider, params object[] constructorParameters)
        {
            Type messageType = messageTypeDefinition.MakeGenericType(messageTypeArgument);
            return (IMessage)constructorProvider(messageType).Invoke(constructorParameters);
        }
    }
}