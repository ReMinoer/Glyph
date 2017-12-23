using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glyph.Messaging
{
    public class Router : IRouter
    {
        private readonly List<InterpreterHandler> _handlers = new List<InterpreterHandler>();

        public int Count => _handlers.Count;
        protected Type InterpreterGenericDefinition { get; }

        public Router()
            : this(typeof(IInterpreter<>))
        {
        }

        public Router(Type interpreterGenericDefinition)
        {
            InterpreterGenericDefinition = interpreterGenericDefinition;
        }

        public virtual void Send(IMessage message)
        {
            if (message.IsHandled)
                return;

            Type messageType = message.GetType();
            foreach (InterpreterHandler handler in _handlers)
                if (handler.TrySend(messageType, message) && message.IsHandled)
                    break;
        }

        public virtual void Register(IInterpreter item)
        {
            if (!_handlers.Any(x => x.Matches(item)))
                _handlers.Add(new InterpreterHandler(item, InterpreterGenericDefinition));
        }

        public virtual bool Unregister(IInterpreter item)
        {
            int index = _handlers.FindIndex(x => x.Matches(item));
            if (index < 0)
                return false;

            _handlers.RemoveAt(index);
            return true;
        }

        public virtual void Clear()
        {
            _handlers.Clear();
        }

        public virtual void ClearDisposed()
        {
            foreach (int index in _handlers.Where(x => !x.WeakReference.TryGetTarget(out IInterpreter _)).Select((x, i) => i).Reverse().ToArray())
                _handlers.RemoveAt(index);
        }

        public bool Contains(IInterpreter item)
        {
            return _handlers.Any(x => x.Matches(item));
        }

        public IEnumerator<IInterpreter> GetEnumerator()
        {
            IEnumerable<IInterpreter> GetInterpreterEnumerable()
            {
                foreach (InterpreterHandler handler in _handlers)
                {
                    if (handler.WeakReference.TryGetTarget(out IInterpreter interpreter))
                        yield return interpreter;
                }
            }
            
            return GetInterpreterEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        private class InterpreterHandler
        {
            private readonly Dictionary<Type, MethodInfo> _delegates = new Dictionary<Type, MethodInfo>();
            public WeakReference<IInterpreter> WeakReference { get; }

            public InterpreterHandler(IInterpreter interpreter, Type interpreterGenericDefinition)
            {
                WeakReference = new WeakReference<IInterpreter>(interpreter);

                foreach (Type interpreterType in interpreter.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interpreterGenericDefinition))
                {
                    Type interpreterMessageType = interpreterType.GenericTypeArguments[0];
                    MethodInfo interpretMethod = interpreterType.GetMethod(nameof(IInterpreter<IMessage>.Interpret), new[] { interpreterMessageType });
                    if (interpretMethod == null)
                        continue;
                    
                    _delegates.Add(interpreterMessageType, interpretMethod);
                }
            }

            public bool Matches(IInterpreter interpreter) => WeakReference.TryGetTarget(out IInterpreter target) && interpreter == target;

            public bool TrySend(Type messageType, IMessage message)
            {
                if (!WeakReference.TryGetTarget(out IInterpreter interpreter))
                    return false;

                bool result = false;
                foreach (MethodInfo method in _delegates.Where(x => x.Key.IsAssignableFrom(messageType)).Select(x => x.Value))
                {
                    method.Invoke(interpreter, new object[]{ message });
                    result = true;
                    
                    if (message.IsHandled)
                        return true;
                }

                return result;
            }
        }
    }
}