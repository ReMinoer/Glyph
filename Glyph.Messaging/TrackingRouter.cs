using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;

namespace Glyph.Messaging
{
    public class TrackingRouter : SubscribeRouter, ITrackingRouter
    {
        private readonly Dictionary<object, List<Delegate>> _subscribersDelegates = new Dictionary<object, List<Delegate>>();
        protected Type InterpreterGenericDefinition { get; }
        protected string InterpretMethodName { get; }

        public TrackingRouter()
            : this(typeof(IInterpreter<>), nameof(IInterpreter<IMessage>.Interpret))
        {
        }

        public TrackingRouter(Type interpreterGenericDefinition, string interpretMethodName)
        {
            InterpreterGenericDefinition = interpreterGenericDefinition;
            InterpretMethodName = interpretMethodName;
        }

        public void Register(object item)
        {
            if (!_subscribersDelegates.TryGetValue(item, out List<Delegate> subscriberDelegates))
            {
                subscriberDelegates = new List<Delegate>();
                _subscribersDelegates.Add(item, subscriberDelegates);
            }

            IEnumerable<Delegate> delegates = item.GetType().GetInterfaces()
                                                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == InterpreterGenericDefinition)
                                                        .Select(i => Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(i.GenericTypeArguments), item, i.GetMethod(InterpretMethodName, i.GenericTypeArguments) ?? throw new InvalidOperationException()));

            foreach (Delegate d in delegates)
            {
                base.Add(d);
                subscriberDelegates.Add(d);
            }
        }

        public void Register<T>(object item)
            where T : IMessage => InternalRegister(item, typeof(T));
        public void Register(object item, Type messageType)
        {
            if (!typeof(IMessage).IsAssignableFrom(messageType))
                throw new ArgumentException();

            InternalRegister(item, messageType);
        }

        private void InternalRegister(object item, Type messageType)
        {
            if (!_subscribersDelegates.TryGetValue(item, out List<Delegate> subscriberDelegates))
            {
                subscriberDelegates = new List<Delegate>();
                _subscribersDelegates.Add(item, subscriberDelegates);
            }

            if (!item.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == InterpreterGenericDefinition
                                                                         && i.GenericTypeArguments[0] == messageType, out Type interpreterType))
                throw new ArgumentException();

            Delegate d = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(interpreterType.GenericTypeArguments), item,
                interpreterType.GetMethod(InterpretMethodName, interpreterType.GenericTypeArguments) ?? throw new InvalidOperationException());
            
            base.Add(d);
            subscriberDelegates.Add(d);
        }

        public bool Unregister(object item)
        {
            if (!_subscribersDelegates.TryGetValue(item, out List<Delegate> subscriberDelegates))
                return false;

            foreach (Delegate subscribeDelegate in subscriberDelegates)
                base.Remove(subscribeDelegate);

            _subscribersDelegates.Remove(item);
            return true;
        }

        public bool Unregister<T>(object item)
            where T : IMessage => InternalUnregister(item, typeof(T));
        public bool Unregister(object item, Type messageType)
        {
            if (!typeof(IMessage).IsAssignableFrom(messageType))
                throw new ArgumentException();

            return InternalUnregister(item, messageType);
        }

        public bool InternalUnregister(object item, Type messageType)
        {
            if (!_subscribersDelegates.TryGetValue(item, out List<Delegate> subscriberDelegates))
                return false;

            if (!subscriberDelegates.Any(x => x.Method.GetParameters()[0].ParameterType == messageType, out Delegate d))
                return false;

            base.Remove(d);
            subscriberDelegates.Remove(d);
            if (subscriberDelegates.Count == 0)
                _subscribersDelegates[item] = null;

            return true;
        }

        public override void Add(Delegate item)
        {
            base.Add(item);
            if (item?.Target == null)
                return;

            if (!_subscribersDelegates.TryGetValue(item.Target, out List<Delegate> subscriberDelegates))
            {
                subscriberDelegates = new List<Delegate>();
                _subscribersDelegates.Add(item.Target, subscriberDelegates);
            }

            subscriberDelegates.Add(item);
        }

        public override bool Remove(Delegate item)
        {
            if (!base.Remove(item))
                return false;

            if (item?.Target != null && _subscribersDelegates.TryGetValue(item.Target, out List<Delegate> subscriberDelegates))
            {
                subscriberDelegates.Remove(item);
                if (subscriberDelegates.Count == 0)
                    _subscribersDelegates[item.Target] = null;
            }

            return true;
        }

        public override void Clear()
        {
            _subscribersDelegates.Clear();
            base.Clear();
        }

        public void ClearDisposed()
        {
            foreach (Delegate subscribeDelegate in _subscribersDelegates.Values.SelectMany(x => x))
                Remove(subscribeDelegate);

            _subscribersDelegates.Clear();
        }

        public bool Contains(object item)
        {
            return _subscribersDelegates.ContainsKey(item);
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _subscribersDelegates.Keys.GetEnumerator();
        }
    }
}