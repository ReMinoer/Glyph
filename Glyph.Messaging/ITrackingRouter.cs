using System;
using Diese.Collections;

namespace Glyph.Messaging
{
    public interface ITrackingRouter : ISubscribableRouter, ITracker<object>
    {
        void Register<T>(object item)
            where T : IMessage;
        bool Unregister<T>(object item)
            where T : IMessage;
        void Register(object item, Type messageType);
        bool Unregister(object item, Type messageType);
    }
}