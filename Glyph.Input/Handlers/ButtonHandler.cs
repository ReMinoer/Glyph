using System.Collections.Generic;
using Glyph.Input.Behaviours;

namespace Glyph.Input.Handlers
{
    public abstract class ButtonHandler : IInputHandler<InputActivity>
    {
        public string Name { get; set; }
        public bool IsActivated { get; protected set; }
        public InputActivity Value { get; private set; }
        public InputActivity DesiredActivity { get; private set; }

        public abstract InputSource InputSource { get; }

        protected readonly ButtonBehaviour ButtonBehaviour = new ButtonBehaviour();

        protected ButtonHandler(string name, InputActivity desiredActivity)
        {
            Name = name;
            DesiredActivity = desiredActivity;
        }

        public void Update(InputStates inputStates)
        {
            bool activity = GetActivity(inputStates);

            Value = ButtonBehaviour.Update(activity);
            IsActivated = Value == DesiredActivity;
        }

        protected abstract bool GetActivity(InputStates inputStates);

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