using System;
using System.Collections.Generic;
using Fingear;
using Fingear.Controls.Base;
using Fingear.MonoGame;
using Fingear.Utils;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Core.Inputs
{
    public enum CursorSpace
    {
        Window,
        Screen,
        VirtualScreen,
        Scene
    }

    public class ReferentialCursorControl : ControlBase<System.Numerics.Vector2>
    {
        private System.Numerics.Vector2? _lastValue;
        private readonly InputClientManager _inputClientManager;
        public ICursorInput Input { get; set; }
        public CursorSpace CursorSpace { get; set; }

        public override IEnumerable<IInput> Inputs
        {
            get { yield return Input; }
        }

        public ReferentialCursorControl(InputClientManager inputClientManager)
        {
            _inputClientManager = inputClientManager;
        }

        public ReferentialCursorControl(InputClientManager inputClientManager, ICursorInput input, CursorSpace cursorSpace)
            : this(inputClientManager)
        {
            Input = input;
            CursorSpace = cursorSpace;
        }

        public ReferentialCursorControl(InputClientManager inputClientManager, string name, ICursorInput input, CursorSpace cursorSpace)
            : this(inputClientManager, input, cursorSpace)
        {
            Name = name;
        }

        protected override bool UpdateControl(float elapsedTime, out System.Numerics.Vector2 value)
        {
            if (Input == null)
            {
                value = default(System.Numerics.Vector2);
                return false;
            }

            System.Numerics.Vector2 state = Input.Value;
            switch (CursorSpace)
            {
                case CursorSpace.Window:
                    value = state;
                    break;
                case CursorSpace.Screen:
                    value = _inputClientManager.Current.Resolution.WindowToScreen(state.AsMonoGamePoint()).AsSystemVector();
                    break;
                case CursorSpace.VirtualScreen:
                    value = _inputClientManager.Current.Resolution.WindowToVirtualScreen(state.AsMonoGamePoint()).AsSystemVector();
                    break;
                case CursorSpace.Scene:
                    Vector2 virtualPosition = _inputClientManager.Current.Resolution.WindowToVirtualScreen(state.AsMonoGamePoint());
                    IView view = ViewManager.Main.GetViewAtPoint(virtualPosition, _inputClientManager.Current as IDrawClient, out Vector2 viewPosition);
                    if (view == null)
                    {
                        value = default(System.Numerics.Vector2);
                        return false;
                    }
                    value = view.ViewToScene(viewPosition).AsSystemVector();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            bool isTriggered = value != _lastValue;
            _lastValue = value;
            return isTriggered;
        }
    }
}