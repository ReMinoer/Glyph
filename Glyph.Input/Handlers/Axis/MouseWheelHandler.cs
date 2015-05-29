namespace Glyph.Input.Handlers.Axis
{
    public class MouseWheelHandler : AxisHandler
    {
        private int _previousState;

        public override InputSource InputSource
        {
            get { return InputSource.Mouse; }
        }

        public MouseWheelHandler()
            : this("")
        {
        }

        public MouseWheelHandler(string name, float deadZone = 0, AxisSign sign = AxisSign.None,
            InputActivity desiredActivity = InputActivity.Pressed)
            : base(name, deadZone, sign, desiredActivity)
        {
        }

        protected override float GetState(InputStates inputStates)
        {
            int state = inputStates.MouseState.ScrollWheelValue - _previousState;
            _previousState = state;
            return state;
        }
    }
}