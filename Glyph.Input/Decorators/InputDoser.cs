using Glyph.Input.Behaviours;

namespace Glyph.Input.Decorators
{
    public class InputDoser : InputDecorator<InputActivity, IInputHandler>
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

        public InputDoser(string name = "", IInputHandler component = null)
            : base(name, component)
        {
        }

        public override void Update(InputStates inputStates)
        {
            base.Update(inputStates);

            _value = _buttonBehaviour.Update(Component.IsActivated);
            _isActivated = _value != InputActivity.None;
        }
    }
}