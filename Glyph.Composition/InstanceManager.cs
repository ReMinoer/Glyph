using System;
using System.Collections.Generic;
using Diese.Injection;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Composition
{
    static public class InstanceManager
    {
        static private readonly Dictionary<Type, GlyphTypeInfo> Dictionary = new Dictionary<Type, GlyphTypeInfo>();
        static public IDependencyInjector RouterInjector { get; set; }

        static public void ConstructorProcess(object instance)
        {
            Type type = instance.GetType();

            if (!Dictionary.ContainsKey(type))
                Dictionary.Add(type, new GlyphTypeInfo(type));

            SendInstanceMessage(typeof(InstantiatingMessage<>), instance, type);
        }

        static public void DisposeProcess(object instance)
        {
            SendInstanceMessage(typeof(DisposingMessage<>), instance, instance.GetType());
        }

        static public GlyphTypeInfo GetInfo(Type type)
        {
            return Dictionary[type];
        }

        static private void SendInstanceMessage(Type messageTypeDefinition, object instance, Type type)
        {
            if (RouterInjector != null)
            {
                Type messageType = messageTypeDefinition.MakeGenericType(type);
                Type routerType = typeof(IRouter<>).MakeGenericType(messageType);

                object router;
                if (RouterInjector.TryResolve(out router, routerType))
                {
                    object message = messageType.GetConstructor(type.AsArray()).Invoke(instance.AsArray());
                    routerType.GetMethod("Send").Invoke(router, message.AsArray());
                }
            }
        }

        static private T[] AsArray<T>(this T obj)
        {
            return new []
            {
                obj
            };
        }
    }
}