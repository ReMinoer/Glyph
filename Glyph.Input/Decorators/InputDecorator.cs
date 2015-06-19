using Diese.Composition;

namespace Glyph.Input.Decorators
{
    public abstract class InputDecorator<TValue, TComponent> : Decorator<IInputHandler, IInputParent, TComponent>,
        IInputDecorator<TValue, TComponent>
        where TComponent : class, IInputHandler
    {
        public virtual InputSource InputSource
        {
            get { return Component != null ? Component.InputSource : InputSource.None; }
        }

        public abstract bool IsActivated { get; }
        public abstract TValue Value { get; }

        protected InputDecorator(string name, TComponent component)
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