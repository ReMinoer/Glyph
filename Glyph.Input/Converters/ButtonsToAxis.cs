using System.Collections.Generic;

namespace Glyph.Input.Converters
{
    public class ButtonsToAxis : InputConverter<InputActivity, float>
    {
        public IInputHandler<InputActivity> PositiveButton
        {
            get { return Components[0]; }
            set { Components[0] = value; }
        }

        public IInputHandler<InputActivity> NegativeButton
        {
            get { return Components[1]; }
            set { Components[1] = value; }
        }

        public ButtonsToAxis(string name = "")
            : base(name, 2)
        {
        }

        public override InputSource InputSource
        {
            get
            {
                if (PositiveButton != null)
                    return PositiveButton.InputSource;
                if (NegativeButton != null)
                    return NegativeButton.InputSource;

                return InputSource.None;
            }
        }

        protected override void HandleInput(IEnumerable<IInputHandler<InputActivity>> components)
        {
            if (!PositiveButton.IsActivated && !NegativeButton.IsActivated)
            {
                IsActivated = false;
                Value = 0;
                return;
            }

            if (PositiveButton.IsActivated)
                Value = 1;
            else if (NegativeButton.IsActivated)
                Value = -1;

            IsActivated = true;
        }
    }
}