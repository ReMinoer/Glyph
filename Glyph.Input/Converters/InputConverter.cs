using System.Collections.Generic;

namespace Glyph.Input.Converters
{
    public abstract class InputConverter<TInput, TOutput> : IInputConverter<TInput, TOutput>
    {
        public string Name { get; set; }
        public IInputHandler<TInput>[] Components { get; private set; }

        public bool IsActivated { get; protected set; }
        public TOutput Value { get; protected set; }
        public abstract InputSource InputSource { get; }

        protected InputConverter(string name, int numberOfComponents)
        {
            Name = name;
            Components = new IInputHandler<TInput>[numberOfComponents];
        }

        public void Update(InputStates inputStates)
        {
            foreach (IInputHandler<TInput> component in Components)
                component.Update(inputStates);

            for (int i = 0; i < Components.Length; i++)
                if (Components[i] == null)
                {
                    IsActivated = false;
                    Value = default(TOutput);
                    return;
                }

            HandleInput(Components);
        }

        protected abstract void HandleInput(IEnumerable<IInputHandler<TInput>> components);

        public T GetComponent<T>(bool includeItself = false)
            where T : class, IInputHandler
        {
            if (includeItself && this is T)
                return this as T;

            foreach (IInputHandler<TInput> inputHandler in Components)
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

            foreach (IInputHandler<TInput> inputHandler in Components)
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

            foreach (IInputHandler<TInput> inputHandler in Components)
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

            foreach (IInputHandler<TInput> inputHandler in Components)
                result.AddRange(inputHandler.GetAllComponentsInChildren<T>());

            return result;
        }
    }
}