namespace Glyph.Input.Behaviours
{
    public class ButtonBehaviour
    {
        private bool _previousState;
        public InputAction Action { get; private set; }

        public InputAction Update(bool state)
        {
            if (state && !_previousState)
                Action = InputAction.Triggered;
            else if (!state && _previousState)
                Action = InputAction.Released;
            else if (state)
                Action = InputAction.Pressed;
            else
                Action = InputAction.None;

            _previousState = state;
            return Action;
        }
    }
}