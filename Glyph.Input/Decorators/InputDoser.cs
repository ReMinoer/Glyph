using Glyph.Input.Behaviours;

namespace Glyph.Input.Decorators
{
    public class InputDoser : InputDecorator<InputActivity, IInputHandler>
    {
        private readonly ButtonBehaviour _buttonBehaviour = new ButtonBehaviour();

        public override bool IsActivated
        {
            get { return Value != InputActivity.None; }
        }

        public override InputActivity Value
        {
            get { return _buttonBehaviour.Activity; }
        }

        public InputDoser(string name = "", IInputHandler component = null)
            : base(name, component)
        {
        }

        public override void Update(InputStates inputStates)
        {
            base.Update(inputStates);

            _buttonBehaviour.Update(Component != null && Component.IsActivated);
        }
    }
}