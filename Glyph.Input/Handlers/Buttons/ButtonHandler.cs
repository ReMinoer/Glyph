using Glyph.Input.Behaviours;

namespace Glyph.Input.Handlers.Buttons
{
    public abstract class ButtonHandler : InputHandler<InputAction>
    {
        private readonly ButtonBehaviour _buttonBehaviour = new ButtonBehaviour();
        public override bool IsActivated { get; protected set; }
        public override InputAction Value { get; protected set; }
        public InputAction DesiredAction { get; private set; }

        protected ButtonHandler(string name, InputAction desiredAction)
            : base(name)
        {
            DesiredAction = desiredAction;
        }

        public override void Update(InputManager inputManager)
        {
            bool state = GetState(inputManager);

            Value = _buttonBehaviour.Update(state);
            IsActivated = Value == DesiredAction;
        }

        protected abstract bool GetState(InputManager inputManager);
    }
}