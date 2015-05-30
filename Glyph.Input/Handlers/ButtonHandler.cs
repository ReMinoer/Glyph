using Diese.Composition;
using Glyph.Input.Behaviours;

namespace Glyph.Input.Handlers
{
    public abstract class ButtonHandler : Component<IInputHandler>, IInputHandler<InputActivity>
    {
        protected readonly ButtonBehaviour ButtonBehaviour = new ButtonBehaviour();
        public bool IsActivated { get; protected set; }
        public InputActivity Value { get; private set; }
        public InputActivity DesiredActivity { get; private set; }
        public abstract InputSource InputSource { get; }

        protected ButtonHandler(string name, InputActivity desiredActivity)
        {
            Name = name;
            DesiredActivity = desiredActivity;
        }

        protected abstract bool GetActivity(InputStates inputStates);

        public void Update(InputStates inputStates)
        {
            bool activity = GetActivity(inputStates);

            Value = ButtonBehaviour.Update(activity);
            IsActivated = Value == DesiredActivity;
        }
    }
}