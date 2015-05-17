namespace Glyph.Input.Composites
{
    public class InputSimultaneous : InputComposite
    {
        public override bool IsTriggered
        {
            get { return _isTriggered; }
        }

        public override float Value
        {
            get { return _value; }
        }

        private bool _isTriggered;
        private float _value;

        public override InputSource InputSource
        {
            get { return Count == 0 ? InputSource.None : Components[0].InputSource; }
        }

        public InputSimultaneous(string name)
            : base(name)
        {
        }

        public override void Update(InputManager inputManager)
        {
            _isTriggered = false;
            _value = 0;

            if (Count == 0)
                return;

            foreach (IInputHandler inputHandler in this)
                inputHandler.Update(inputManager);

            foreach (IInputHandler inputHandler in this)
                if (!inputHandler.IsTriggered)
                    return;

            _isTriggered = true;
            _value = Components[0].Value;
        }
    }
}
