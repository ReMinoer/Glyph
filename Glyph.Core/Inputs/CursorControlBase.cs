using System.Collections.Generic;
using Fingear;
using Fingear.Controls.Base;

namespace Glyph.Core.Inputs
{
    public abstract class CursorControlBase : ControlBase<System.Numerics.Vector2>
    {
        private System.Numerics.Vector2? _lastValue;
        public ICursorInput Input { get; set; }

        public override sealed IEnumerable<IInput> Inputs
        {
            get { yield return Input; }
        }

        protected CursorControlBase(string name, ICursorInput input)
        {
            Name = name;
            Input = input;
        }

        protected override sealed bool UpdateControlValue(float elapsedTime, out System.Numerics.Vector2 value)
        {
            if (Input == null || !ComputeCursor(out value))
            {
                value = default;
                _lastValue = value;
                return false;
            }

            bool isTriggered = value != _lastValue;
            _lastValue = value;
            return isTriggered;
        }

        protected abstract bool ComputeCursor(out System.Numerics.Vector2 value);
    }
}