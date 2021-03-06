﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class ComponentRouterSystem : ITrackingRouter
    {
        static private readonly TrackingRouterNull RouterNull = new TrackingRouterNull();

        private readonly IGlyphComponent _component;

        private ITrackingRouter _global = RouterNull;
        public ITrackingRouter Global
        {
            get => _global;
            set
            {
                value = value ?? RouterNull;
                if (_global == value)
                    return;
                
                _global.Unregister(_component);
                _global = value;
                _global.Register(_component);

                foreach (IGlyphComponent component in _component.Components)
                    component.Router.Global = value;
            }
        }

        private ISubscribableRouter GlobalRouter => Global;

        public TrackingRouter Local { get; } = new TrackingRouter(typeof(ILocalInterpreter<>), nameof(ILocalInterpreter<IMessage>.Interpret));
        public IRouter Ascending { get; }
        public IRouter Descending { get; }

        public bool IsReady => Global != RouterNull;
        public int Count => GlobalRouter.Count + Local.Count;

        public ComponentRouterSystem(IGlyphComponent component)
        {
            _component = component;
            
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

        public bool Register(object item)
        {
            return Global.Register(item) || Local.Register(item);
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
        int ITracker<object>.Count => GlobalRouter.Count + Local.Count;

        void ICollection<Delegate>.Clear()
        {
            GlobalRouter.Clear();
            Local.Clear();
        }

        void ICollection<Delegate>.CopyTo(Delegate[] array, int arrayIndex)
        {
            Global.CopyTo(array, arrayIndex);
            Local.CopyTo(array, arrayIndex + GlobalRouter.Count);
        }

        void ITracker<object>.Clear()
        {
            GlobalRouter.Clear();
            Local.Clear();
        }

        public IEnumerator<Delegate> GetEnumerator()
        {
            return Global.Concat((IEnumerable<Delegate>)Local).GetEnumerator();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator() => Global.Concat((IEnumerable<object>)Local).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Global.Concat(Local).GetEnumerator();
    }
}