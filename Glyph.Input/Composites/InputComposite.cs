using Diese.Composition;

namespace Glyph.Input.Composites
{
    public abstract class InputComposite<TValue> : Composite<IInputHandler, IInputParent>, IInputComposite<TValue>
    {
        public InputActivity DesiredActivity { get; set; }
        public abstract bool IsActivated { get; }
        public abstract TValue Value { get; }
        public abstract InputSource InputSource { get; }

        protected InputComposite(string name)
        {
            Name = name;
        }

        protected abstract void HandleInput(InputStates inputStates);

        public virtual void Update(InputStates inputStates)
        {
            foreach (IInputHandler inputHandler in this)
                inputHandler.Update(inputStates);

            HandleInput(inputStates);
        }
    }
}