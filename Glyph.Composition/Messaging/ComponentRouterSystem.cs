using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class ComponentRouterSystem : ITrackingRouter
    {
        static public TrackingRouter GlobalRouter { get; } = new TrackingRouter();
        public TrackingRouter Global => GlobalRouter;
        public TrackingRouter Local { get; } = new TrackingRouter(typeof(ILocalInterpreter<>), nameof(ILocalInterpreter<IMessage>.Interpret));
        public IRouter Ascending { get; }
        public IRouter Descending { get; }

        public int Count => Global.Count + Local.Count;

        public ComponentRouterSystem(IGlyphComponent component)
        {
            Global.Register(component);
            Local.Register(component);

            Ascending = new AscendingRouter(component);
            Descending = new DescendingRouter(component);
        }

        private class AscendingRouter : IRouter
        {
            private readonly IGlyphComponent _component;
            public AscendingRouter(IGlyphComponent component) => _component = component;

            public void Send(IMessage message)
            {
                IGlyphContainer parent = _component.Parent;
                if (parent == null)
                    return;

                parent.Router.Local.Send(message);
                parent.Router.Ascending.Send(message);
            }
        }

        private class DescendingRouter : IRouter
        {
            private readonly IGlyphComponent _component;
            public DescendingRouter(IGlyphComponent component) => _component = component;

            public void Send(IMessage message)
            {
                foreach (IGlyphComponent child in _component.Components)
                    child.Router.Local.Send(message);

                foreach (IGlyphComponent child in _component.Components)
                    child.Router.Descending.Send(message);
            }
        }

        public void Send(IMessage message)
        {
            Local.Send(message);
            Ascending.Send(message);
            Descending.Send(message);
            Global.Send(message);
        }

        public void Register(object item)
        {
            Global.Register(item);
            Local.Register(item);
        }

        public void Register<T>(object item)
            where T : IMessage
        {
            Global.Register<T>(item);
            Local.Register<T>(item);
        }

        public void Register(object item, Type messageType)
        {
            Global.Register(item, messageType);
            Local.Register(item, messageType);
        }

        public bool Unregister(object item)
        {
            return Global.Unregister(item) | Local.Unregister(item);
        }

        public bool Unregister<T>(object item)
            where T : IMessage
        {
            return Global.Unregister<T>(item) | Local.Unregister<T>(item);
        }

        public bool Unregister(object item, Type messageType)
        {
            return Global.Unregister(item, messageType) | Local.Unregister(item, messageType);
        }

        public void Add(Delegate item)
        {
            Global.Add(item);
            Local.Add(item);
        }

        public bool Remove(Delegate item)
        {
            return Global.Remove(item) | Local.Remove(item);
        }

        public bool Contains(object item)
        {
            return Global.Contains(item) || Local.Contains(item);
        }

        public bool Contains(Delegate item)
        {
            return Global.Contains(item) || Local.Contains(item);
        }

        public void Add<T>(Action<T> item)
            where T : IMessage
        {
            Global.Add(item);
            Local.Add(item);
        }

        public bool Remove<T>(Action<T> item)
            where T : IMessage
        {
            return Global.Remove(item) | Local.Remove(item);
        }

        public bool Contains<T>(Action<T> item)
            where T : IMessage
        {
            return Global.Contains(item) || Local.Contains(item);
        }

        bool ICollection<Delegate>.IsReadOnly => false;
        int ITracker<object>.Count => Global.Count + Local.Count;

        void ICollection<Delegate>.Clear()
        {
            Global.Clear();
            Local.Clear();
        }

        void ICollection<Delegate>.CopyTo(Delegate[] array, int arrayIndex)
        {
            Global.CopyTo(array, arrayIndex);
            Local.CopyTo(array, arrayIndex + Global.Count);
        }

        void ITracker<object>.Clear()
        {
            Global.Clear();
            Local.Clear();
        }

        void ITracker<object>.ClearDisposed()
        {
            Global.ClearDisposed();
            Local.ClearDisposed();
        }

        public IEnumerator<Delegate> GetEnumerator()
        {
            return Global.Concat((IEnumerable<Delegate>)Local).GetEnumerator();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator() => Global.Concat((IEnumerable<object>)Local).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Global.Concat(Local).GetEnumerator();
    }
}