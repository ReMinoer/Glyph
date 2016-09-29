using System;
using System.Collections.Generic;
using Fingear;
using Fingear.Controls.Base;
using Fingear.MonoGame;
using Fingear.Utils;
using Glyph.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Input.Controls
{
    public enum CursorSpace
    {
        Window,
        Screen,
        VirtualScreen,
        Scene
    }

    public class ReferentialCursorControl : ControlBase<Fingear.Vector2>
    {
        public ICursorInput Input { get; set; }
        public CursorSpace CursorSpace { get; set; }
        public override IEnumerable<IInputSource> Sources => Input?.Source.ToEnumerable();

        public override IEnumerable<IInput> Inputs
        {
            get { yield return Input; }
        }

        public ReferentialCursorControl()
        {
        }

        public ReferentialCursorControl(ICursorInput input, CursorSpace cursorSpace)
        {
            Input = input;
            CursorSpace = cursorSpace;
        }

        public ReferentialCursorControl(string name, ICursorInput input, CursorSpace cursorSpace)
            : this(input, cursorSpace)
        {
            Name = name;
        }

        protected override bool UpdateControl(float elapsedTime, out Fingear.Vector2 value)
        {
            if (Input == null)
            {
                value = default(Fingear.Vector2);
                return false;
            }

            Fingear.Vector2 state = Input.Value;
            switch (CursorSpace)
            {
                case CursorSpace.Window:
                    value = state;
                    break;
                case CursorSpace.Screen:
                    value = Resolution.Instance.WindowToScreen(state.AsMonoGamePoint()).AsFingearVector();
                    break;
                case CursorSpace.VirtualScreen:
                    value = Resolution.Instance.WindowToVirtualScreen(state.AsMonoGamePoint()).AsFingearVector();
                    break;
                case CursorSpace.Scene:
                    Vector2 viewPosition;
                    IView view = ViewManager.GetView(state.AsMonoGamePoint(), out viewPosition);
                    value = view.ViewToScene(viewPosition).AsFingearVector();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return true;
        }
    }
}