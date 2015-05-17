using System.Collections.Generic;

namespace Glyph.Input
{
    public abstract class InputHandler : IInputHandler
    {
        public string Name { get; private set; }

        protected InputHandler(string name)
        {
            Name = name;
        }

        public abstract bool IsTriggered { get; protected set; }
        public abstract float Value { get; protected set; }
        public abstract InputSource InputSource { get; }
        public abstract void Update(InputManager inputManager);

        public T GetComponent<T>(bool includeItself = false) where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return this as T;
            return null;
        }

        public List<T> GetAllComponents<T>(bool includeItself = false) where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return new List<T> { this as T };

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