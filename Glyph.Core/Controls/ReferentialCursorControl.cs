using System;
using System.Collections.Generic;
using Fingear;
using Fingear.Controls.Base;
using Fingear.MonoGame;
using Fingear.Utils;
using Glyph.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Core.Controls
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
        private readonly ControlManager _controlManager;
        public ICursorInput Input { get; set; }
        public CursorSpace CursorSpace { get; set; }
        public override IEnumerable<IInputSource> Sources => Input?.Source.ToEnumerable();

        public override IEnumerable<IInput> Inputs
        {
            get { yield return Input; }
        }

        public ReferentialCursorControl(ControlManager controlManager)
        {
            _controlManager = controlManager;
        }

        public ReferentialCursorControl(ControlManager controlManager, ICursorInput input, CursorSpace cursorSpace)
            : this(controlManager)
        {
            Input = input;
            CursorSpace = cursorSpace;
        }

        public ReferentialCursorControl(ControlManager controlManager, string name, ICursorInput input, CursorSpace cursorSpace)
            : this(controlManager, input, cursorSpace)
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
                    value = _controlManager.InputClient.Resolution.WindowToScreen(state.AsMonoGamePoint()).AsSystemVector();
                    break;
                case CursorSpace.VirtualScreen:
                    value = _controlManager.InputClient.Resolution.WindowToVirtualScreen(state.AsMonoGamePoint()).AsSystemVector();
                    break;
                case CursorSpace.Scene:
                    Vector2 virtualPosition = _controlManager.InputClient.Resolution.WindowToVirtualScreen(state.AsMonoGamePoint());
                    IView view = ViewManager.Main.GetViewAtPoint(virtualPosition, out Vector2 viewPosition);
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
            
            return !Input.Activity.IsIdle();
        }
    }
}