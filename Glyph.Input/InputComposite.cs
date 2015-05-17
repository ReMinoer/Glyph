using System.Collections;
using System.Collections.Generic;

namespace Glyph.Input
{
    public abstract class InputComposite : IInputComposite
    {
        public string Name { get; private set; }
        public abstract bool IsTriggered { get; }
        public abstract float Value { get; }
        public abstract InputSource InputSource { get; }
        protected readonly List<IInputHandler> Components;

        protected InputComposite(string name)
        {
            Name = name;
            Components = new List<IInputHandler>();
        }
        public abstract void Update(InputManager inputManager);

        public T GetComponent<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return this as T;

            foreach (IInputHandler inputHandler in this)
                if (inputHandler is T)
                    return inputHandler as T;

            return null;
        }

        public List<T> GetAllComponents<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            var result = new List<T>();

            if (includeItself && this is T)
                result.Add(this as T);

            foreach (IInputHandler inputHandler in this)
                if (inputHandler is T)
                    result.Add(inputHandler as T);

            return result;
        }

        public T GetComponentInChildren<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            var component = GetComponent<T>(includeItself);
            if (component != null)
                return component;
            
            foreach (IInputHandler inputHandler in this)
                if (inputHandler is IInputComposite)
                {
                    T first = (inputHandler as IInputComposite).GetComponentInChildren<T>();
                    if (first != null)
                        return first;
                }

            return null;
        }

        public List<T> GetAllComponentsInChildren<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            var result = GetAllComponents<T>(includeItself);

            foreach (IInputHandler inputHandler in this)
                if (inputHandler is IInputComposite)
                    result.AddRange((inputHandler as IInputComposite).GetAllComponentsInChildren<T>());

            return result;
        }

        public IEnumerator<IInputHandler> GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        public void Add(IInputHandler item)
        {
            Components.Add(item);
        }

        public void Clear()
        {
            Components.Clear();
        }

        public bool Contains(IInputHandler item)
        {
            return Components.Contains(item);
        }

        public void CopyTo(IInputHandler[] array, int arrayIndex)
        {
            Components.CopyTo(array, arrayIndex);
        }

        public bool Remove(IInputHandler item)
        {
            return Components.Remove(item);
        }

        public int Count
        {
            get { return Components.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}