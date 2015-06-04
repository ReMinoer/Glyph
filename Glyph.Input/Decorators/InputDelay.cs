using Glyph.Input.Behaviours;

namespace Glyph.Input.Decorators
{
    public class InputDelay : InputDecorator<InputActivity, IInputHandler>
    {
        private readonly ButtonBehaviour _buttonBehaviour = new ButtonBehaviour();
        private readonly Period _period = new Period();
        public InputActivity DesiredActivity { get; set; }

        public override bool IsActivated
        {
            get { return Value == DesiredActivity; }
        }

        public override InputActivity Value
        {
            get { return _buttonBehaviour.Activity; }
        }

        public int Period
        {
            get { return _period.Interval; }
            set { _period.Interval = value; }
        }

        public InputDelay()
            : this("", null, 0)
        {
        }

        public InputDelay(string name, IInputHandler component, int period,
            InputActivity desiredActivity = InputActivity.Triggered)
            : base(name, component)
        {
            Period = period;
            DesiredActivity = desiredActivity;
        }

        public override void Update(InputStates inputStates)
        {
            base.Update(inputStates);

            if (Component != null && Component.IsActivated)
                _period.Update(ElapsedTime.Instance);
            else
                _period.Init();

            _buttonBehaviour.Update(_period.IsEnd);
        }
    }
}