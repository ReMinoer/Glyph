namespace Glyph.Input.Handlers
{
    public abstract class SensitiveHandler<TValue> : ButtonHandler, IInputHandler<TValue>
    {
        new public abstract TValue Value { get; protected set; }

        protected SensitiveHandler(string name, InputActivity desiredActivity)
            : base(name, desiredActivity)
        {
        }

        protected abstract void HandleInput(InputStates inputStates);

        new public void Update(InputStates inputStates)
        {
            HandleInput(inputStates);
            base.Update(inputStates);
        }

        protected override sealed bool GetActivity(InputStates inputStates)
        {
            return IsActivated;
        }
    }
}