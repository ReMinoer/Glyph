using System.Collections;
using System.Collections.Generic;

namespace Glyph.Input.Composites
{
    public abstract class InputComposite<TValue> : IInputComposite<TValue>
    {
        protected readonly List<IInputHandler> Components;
        public string Name { get; private set; }
        public abstract bool IsActivated { get; }
        public abstract TValue Value { get; }
        public abstract InputSource InputSource { get; }

        public int Count
        {
            get { return Components.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        protected InputComposite(string name)
        {
            Name = name;
            Components = new List<IInputHandler>();
        }

        public virtual void Update(InputStates inputStates)
        {
            foreach (IInputHandler inputHandler in this)
                inputHandler.Update(inputStates);
        }

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
            {
                var first = inputHandler.GetComponentInChildren<T>();
                if (first != null)
                    return first;
            }

            return null;
        }

        public List<T> GetAllComponentsInChildren<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            List<T> result = GetAllComponents<T>(includeItself);

            foreach (IInputHandler inputHandler in this)
                result.AddRange(inputHandler.GetAllComponentsInChildren<T>());

            return result;
        }

        public IEnumerator<IInputHandler> GetEnumerator()
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Components.GetEnumerator();
        }
    }
}