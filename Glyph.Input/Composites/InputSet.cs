namespace Glyph.Input.Composites
{
    public class InputSet : InputComposite
    {
        public override bool IsTriggered
        {
            get { return _isTriggered; }
        }

        public override float Value
        {
            get { return _value; }
        }

        public override InputSource InputSource
        {
            get { return _inputSource; }
        }

        private InputSource _inputSource;
        private float _value;
        private bool _isTriggered;

        public InputSet(string name)
            : base(name)
        {
        }

        public override void Update(InputManager inputManager)
        {
            _isTriggered = false;
            foreach (IInputHandler inputHandler in this)
            {
                inputHandler.Update(inputManager);
                if (!IsTriggered && inputHandler.IsTriggered)
                {
                    _isTriggered = true;
                    _value = inputHandler.Value;
                    _inputSource = inputHandler.InputSource;
                }
            }
        }
    }
}