using System.Collections.Generic;
using Diese.Composition;

namespace Glyph.Input.Decorators
{
    public abstract class InputDecorator<TValue> : IInputDecorator<TValue>
    {
        public IInputHandler Component { get; set; }
        public string Name { get; set; }

        public virtual InputSource InputSource
        {
            get { return Component != null ? Component.InputSource : InputSource.None; }
        }

        public abstract bool IsActivated { get; }
        public abstract TValue Value { get; }

        protected InputDecorator(string name, IInputHandler component)
        {
            Name = name;
            Component = component;
        }

        public virtual void Update(InputStates inputStates)
        {
            Component.Update(inputStates);
        }

        public T GetComponent<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return this as T;

            return Component as T;
        }

        public List<T> GetAllComponents<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            var result = new List<T>();

            if (includeItself && this is T)
                result.Add(this as T);

            if (Component is T)
                result.Add(Component as T);

            return result;
        }

        public T GetComponentInChildren<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            var component = GetComponent<T>(includeItself);
            if (component != null)
                return component;

            return Component != null ? Component.GetComponentInChildren<T>() : null;
        }

        public List<T> GetAllComponentsInChildren<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            List<T> result = GetAllComponents<T>(includeItself);

            if (Component != null)
                result.AddRange(Component.GetAllComponentsInChildren<T>());

            return result;
        }

        public bool ContainsComponent(IComponent<IInputHandler> component)
        {
            return Component.Equals(component);
        }

        public bool ContainsComponentInChildren(IComponent<IInputHandler> component)
        {
            if (ContainsComponent(component))
                return true;

            return Component.ContainsComponentInChildren(component);
        }
    }
}