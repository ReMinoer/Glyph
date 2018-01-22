using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Glyph.Messaging
{
    public class SubscribeRouter : ISubscribableRouter
    {
        private readonly List<Delegate> _delegates = new List<Delegate>();

        public int Count => _delegates.Count;
        public bool IsReadOnly => false;

        public void Send(IMessage message)
        {
            if (message.IsHandled)
                return;

            Type messageType = message.GetType();
            foreach (Delegate delegateCall in _delegates)
            {
                Type delegateMessageType = delegateCall.Method.GetParameters()[0].ParameterType;
                if (!delegateMessageType.IsAssignableFrom(messageType))
                    continue;

                delegateCall.DynamicInvoke(message);
                if (message.IsHandled)
                    break;
            }
        }

        public virtual void Add(Delegate item)
        {
            if (item == null)
                throw new ArgumentException();

            ParameterInfo[] parameterInfos = item.Method.GetParameters();
            if (parameterInfos.Length != 1)
                throw new ArgumentException();

            Type messageType = parameterInfos[0].ParameterType;
            if (!typeof(IMessage).IsAssignableFrom(messageType))
                throw new ArgumentException();

            _delegates.Add(item);
        }

        public virtual bool Remove(Delegate item) => _delegates.Remove(item);
        public virtual void Clear() => _delegates.Clear();
        public bool Contains(Delegate item) => _delegates.Contains(item);

        public void Add<T>(Action<T> item)
            where T : IMessage => Add((Delegate)item);
        public bool Remove<T>(Action<T> item)
            where T : IMessage => Remove((Delegate)item);
        public bool Contains<T>(Action<T> item)
            where T : IMessage => Contains((Delegate)item);

        public void CopyTo(Delegate[] array, int arrayIndex) => _delegates.CopyTo(array, arrayIndex);
        public IEnumerator<Delegate> GetEnumerator() => _delegates.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_delegates).GetEnumerator();
    }
}