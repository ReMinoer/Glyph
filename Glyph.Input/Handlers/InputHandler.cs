using System.Collections.Generic;

namespace Glyph.Input.Handlers
{
    public abstract class InputHandler<TValue> : IInputHandler<TValue>
    {
        public string Name { get; private set; }
        public abstract bool IsActivated { get; protected set; }
        public abstract TValue Value { get; protected set; }
        public abstract InputSource InputSource { get; }

        protected InputHandler(string name)
        {
            Name = name;
        }

        public abstract void Update(InputStates inputStates);

        public T GetComponent<T>(bool includeItself = false) where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return this as T;
            return null;
        }

        public List<T> GetAllComponents<T>(bool includeItself = false) where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return new List<T> {this as T};

            return new List<T>();
        }

        public T GetComponentInChildren<T>(bool includeItself = false) where T : class, IInputHandler
        {
            return GetComponent<T>();
        }

        public List<T> GetAllComponentsInChildren<T>(bool includeItself = false) where T : class, IInputHandler
        {
            return GetAllComponents<T>();
        }
    }
}