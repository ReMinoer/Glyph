using System.Numerics;
using Fingear;
using Fingear.MonoGame;

namespace Glyph.Core.Inputs
{
    public class VirtualScreenCursorControl : CursorControlBase
    {
        private readonly InputClientManager _inputClientManager;

        public VirtualScreenCursorControl(InputClientManager inputClientManager)
            : this(null, null, inputClientManager)
        {
        }

        public VirtualScreenCursorControl(ICursorInput input, InputClientManager inputClientManager)
            : this(null, input, inputClientManager)
        {
        }

        public VirtualScreenCursorControl(string name, ICursorInput input, InputClientManager inputClientManager)
            : base(name, input)
        {
            _inputClientManager = inputClientManager;
        }

        protected override bool ComputeCursor(out Vector2 value)
        {
            value = _inputClientManager.Current.Resolution.WindowToVirtualScreen(Input.Value.AsMonoGamePoint()).AsSystemVector();
            return true;
        }
    }
}