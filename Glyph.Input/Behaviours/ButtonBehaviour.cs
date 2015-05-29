namespace Glyph.Input.Behaviours
{
    public class ButtonBehaviour
    {
        private bool _previousState;
        public InputActivity Activity { get; private set; }

        public InputActivity Update(bool state)
        {
            if (state && !_previousState)
                Activity = InputActivity.Triggered;
            else if (!state && _previousState)
                Activity = InputActivity.Released;
            else if (state)
                Activity = InputActivity.Pressed;
            else
                Activity = InputActivity.None;

            _previousState = state;
            return Activity;
        }
    }
}