using System;
using System.Collections.Generic;
using System.Linq;
using Diese;
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

            if (instance is IGlyphComponent)
                SendInstanceMessage(typeof(InstantiatingMessage<>), instance, type);

            foreach (Type inheritedType in instance.GetType().GetInheritedTypes<IGlyphComponent>())
                SendInstanceMessage(typeof(InstantiatingMessage<>), instance, inheritedType);
        }

        static public void DisposeProcess(object instance)
        {
            Type type = instance.GetType();

            if (instance is IGlyphComponent)
                SendInstanceMessage(typeof(DisposingMessage<>), instance, type);

            foreach (Type inheritedType in instance.GetType().GetInheritedTypes<IGlyphComponent>())
                SendInstanceMessage(typeof(DisposingMessage<>), instance, inheritedType);
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
                    object message = messageType.GetConstructor(new [] { type })?.Invoke(new[] { instance });
                    routerType.GetMethod("Send").Invoke(router, new[] { message });
                }
            }
        }
    }
}