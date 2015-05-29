using Glyph.Input.Behaviours;

namespace Glyph.Input.Composites
{
    public class InputSimultaneous : InputComposite<InputActivity>
    {
        private readonly ButtonBehaviour _buttonBehaviour = new ButtonBehaviour();
        private bool _isActivated;
        private InputActivity _value;

        public override bool IsActivated
        {
            get { return _isActivated; }
        }

        public override InputActivity Value
        {
            get { return _value; }
        }

        public override InputSource InputSource
        {
            get { return Count == 0 ? InputSource.None : Components[0].InputSource; }
        }

        public InputSimultaneous(string name = "")
            : base(name)
        {
        }

        protected override void HandleInput(InputStates inputStates)
        {
            if (Count == 0)
            {
                _isActivated = false;
                _value = InputActivity.None;
                return;
            }

            _isActivated = true;
            foreach (IInputHandler inputHandler in this)
                if (!inputHandler.IsActivated)
                {
                    _isActivated = false;
                    break;
                }

            _value = _buttonBehaviour.Update(_isActivated);
        }
    }
}