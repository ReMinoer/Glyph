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

        public MouseWheelHandler(string name)
            : base(name)
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