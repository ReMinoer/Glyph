using Glyph.Input.Behaviours;

namespace Glyph.Input.Composites
{
    public class InputSimultaneous : InputComposite<InputAction>
    {
        private readonly ButtonBehaviour _buttonBehaviour = new ButtonBehaviour();
        private bool _isActivated;
        private InputAction _value;

        public override bool IsActivated
        {
            get { return _isActivated; }
        }

        public override InputAction Value
        {
            get { return _value; }
        }

        public override InputSource InputSource
        {
            get { return Count == 0 ? InputSource.None : Components[0].InputSource; }
        }

        public InputSimultaneous(string name)
            : base(name)
        {
        }

        public override void Update(InputStates inputStates)
        {
            if (Count == 0)
            {
                _isActivated = false;
                _value = InputAction.None;
                return;
            }

            base.Update(inputStates);

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