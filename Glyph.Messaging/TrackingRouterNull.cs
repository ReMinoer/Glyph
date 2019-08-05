using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Glyph.Messaging
{
    public class TrackingRouterNull : ITrackingRouter
    {
        void IRouter.Send(IMessage message) {}

        void ISubscribableRouter.Add<T>(Action<T> item) {}
        bool ISubscribableRouter.Remove<T>(Action<T> item) => false;
        bool ISubscribableRouter.Contains<T>(Action<T> item) => false;
        
        int ICollection<Delegate>.Count => 0;
        bool ICollection<Delegate>.IsReadOnly => false;
        void ICollection<Delegate>.Add(Delegate item) {}
        void ICollection<Delegate>.Clear() {}
        bool ICollection<Delegate>.Contains(Delegate item) => false;
        void ICollection<Delegate>.CopyTo(Delegate[] array, int arrayIndex) {}
        bool ICollection<Delegate>.Remove(Delegate item) => false;
        
        int ITracker<object>.Count => 0;
        void ITracker<object>.Register(object item) {}
        bool ITracker<object>.Unregister(object item) => false;
        void ITracker<object>.Clear() {}
        void ITracker<object>.ClearDisposed() {}
        bool ITracker<object>.Contains(object item) => false;

        void ITrackingRouter.Register<T>(object item) {}
        bool ITrackingRouter.Unregister<T>(object item) => false;
        void ITrackingRouter.Register(object item, Type messageType) {}
        bool ITrackingRouter.Unregister(object item, Type messageType) => false;

        IEnumerator<object> IEnumerable<object>.GetEnumerator() => Enumerable.Empty<object>().GetEnumerator();
        IEnumerator<Delegate> IEnumerable<Delegate>.GetEnumerator() => Enumerable.Empty<Delegate>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Enumerable.Empty<object>().GetEnumerator();
    }
}