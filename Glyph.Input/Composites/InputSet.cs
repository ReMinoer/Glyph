namespace Glyph.Input.Composites
{
    public class InputSet<TValue> : InputComposite<TValue>
    {
        private InputSource _inputSource;
        private bool _isActivated;
        private TValue _value;

        public override bool IsActivated
        {
            get { return _isActivated; }
        }

        public override TValue Value
        {
            get { return _value; }
        }

        public override InputSource InputSource
        {
            get { return _inputSource; }
        }

        public InputSet(string name)
            : base(name)
        {
        }

        public override void Update(InputStates inputStates)
        {
            base.Update(inputStates);

            _isActivated = false;
            foreach (IInputHandler inputHandler in this)
            {
                if (!IsActivated && inputHandler.IsActivated)
                {
                    _isActivated = true;
                    if (inputHandler is IInputHandler<TValue>)
                        _value = (inputHandler as IInputHandler<TValue>).Value;
                    else
                        _value = default(TValue);
                    _inputSource = inputHandler.InputSource;
                }
            }
        }
    }
}