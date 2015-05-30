using Diese.Composition;

namespace Glyph.Input.Decorators
{
    public abstract class InputDecorator<TValue> : Decorator<IInputHandler>, IInputDecorator<TValue>
    {
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
    }
}