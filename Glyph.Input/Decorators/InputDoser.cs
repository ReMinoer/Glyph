using Glyph.Input.Behaviours;

namespace Glyph.Input.Decorators
{
    public class InputDoser : InputDecorator<InputAction>
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

        public InputDoser(IInputHandler component)
            : base(component)
        {
        }

        public override void Update(InputStates inputStates)
        {
            base.Update(inputStates);

            _value = _buttonBehaviour.Update(Component.IsActivated);
            _isActivated = _value != InputAction.None;
        }
    }
}