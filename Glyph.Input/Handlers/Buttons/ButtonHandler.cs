using Glyph.Input.Behaviours;

namespace Glyph.Input.Handlers.Buttons
{
    public abstract class ButtonHandler : InputHandler<InputAction>
    {
        private readonly ButtonBehaviour _buttonBehaviour = new ButtonBehaviour();
        public InputAction DesiredAction { get; set; }
        public override bool IsActivated { get; protected set; }
        public override InputAction Value { get; protected set; }

        protected ButtonHandler(string name, InputAction desiredAction)
            : base(name)
        {
            DesiredAction = desiredAction;
        }

        public override void Update(InputStates inputStates)
        {
            bool state = GetState(inputStates);

            Value = _buttonBehaviour.Update(state);
            IsActivated = Value == DesiredAction;
        }

        protected abstract bool GetState(InputStates inputStates);
    }
}