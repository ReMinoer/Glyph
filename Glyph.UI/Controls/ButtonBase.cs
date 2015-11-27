using System;
using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Controls
{
    public abstract class ButtonBase : GlyphObject, IButton
    {
        private readonly InputManager _inputManager;
        private bool _hover;
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Text Text { get; private set; }
        public bool Pressed { get; private set; }
        public abstract IFrame Frame { get; }

        public bool Hover
        {
            get { return _hover; }
            set
            {
                if (_hover == value)
                    return;

                _hover = value;

                if (Hover && Entered != null)
                    Entered(this, EventArgs.Empty);
                else if (!Hover && Leaved != null)
                    Leaved(this, EventArgs.Empty);
            }
        }

        public event EventHandler Triggered;
        public event EventHandler Released;
        public event EventHandler Entered;
        public event EventHandler Leaved;

        protected ButtonBase(InputManager inputManager, IDependencyInjector injector)
            : base(injector)
        {
            _inputManager = inputManager;

            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            Text = Add<Text>();

            Schedulers.Update.Plan(HandleInput);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            bool hover = Hover;
            if (_inputManager.IsMouseUsed)
            {
                var mouseInScreen = _inputManager.GetValue<Vector2>(MouseInputs.VirtualScreenPosition);
                hover = Frame.Bounds.ContainsPoint(mouseInScreen);
            }

            if (Hover)
            {
                if (_inputManager.IsMouseUsed)
                {
                    if (_inputManager[MenuInputs.Clic])
                        Trigger();
                    else if (Pressed && _inputManager[MenuInputs.ReleaseClic])
                        Release();
                }
                else if (_inputManager[MenuInputs.Confirm])
                {
                    Trigger();
                    Release();
                }
            }
            else if (Pressed)
                Release();

            Hover = hover;
        }

        private void Trigger()
        {
            Pressed = true;
            if (Triggered != null)
                Triggered(this, EventArgs.Empty);
        }

        private void Release()
        {
            Pressed = false;
            if (Released != null)
                Released(this, EventArgs.Empty);
        }
    }
}