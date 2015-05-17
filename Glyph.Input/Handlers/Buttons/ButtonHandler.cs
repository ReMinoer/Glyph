using System;

namespace Glyph.Input.Handlers.Buttons
{
    public abstract class ButtonHandler : InputHandler
    {
        public override bool IsTriggered { get; protected set; }
        public override float Value { get; protected set; }
        public ButtonHandlerMode Mode { get; private set; }

        private bool _previousState;

        protected ButtonHandler(string name, ButtonHandlerMode mode)
            : base(name)
        {
            Mode = mode;
        }

        public override void Update(InputManager inputManager)
        {
            bool state = GetState(inputManager);

            switch (Mode)
            {
                case ButtonHandlerMode.Triggered:
                    IsTriggered = state && !_previousState;
                    break;
                case ButtonHandlerMode.Released:
                    IsTriggered = !state && _previousState;
                    break;
                case ButtonHandlerMode.Pressed:
                    IsTriggered = state;
                    break;
                default:
                    throw new NotImplementedException();
            }

            Value = IsTriggered ? 1 : 0;
            _previousState = state;
        }

        protected abstract bool GetState(InputManager inputManager);
    }
}