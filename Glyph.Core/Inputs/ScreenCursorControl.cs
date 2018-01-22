using System.Numerics;
using Fingear;
using Fingear.MonoGame;

namespace Glyph.Core.Inputs
{
    public class ScreenCursorControl : CursorControlBase
    {
        private readonly InputClientManager _inputClientManager;

        public ScreenCursorControl(InputClientManager inputClientManager)
            : this(null, null, inputClientManager)
        {
        }

        public ScreenCursorControl(ICursorInput input, InputClientManager inputClientManager)
            : this(null, input, inputClientManager)
        {
        }

        public ScreenCursorControl(string name, ICursorInput input, InputClientManager inputClientManager)
            : base(name, input)
        {
            _inputClientManager = inputClientManager;
        }
        
        protected override bool ComputeCursor(out Vector2 value)
        {
            value = _inputClientManager.Current.Resolution.WindowToScreen(Input.Value.AsMonoGamePoint()).AsSystemVector();
            return true;
        }
    }
}