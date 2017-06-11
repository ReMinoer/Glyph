using System;
using System.Linq;
using Diese.Injection;
using Fingear;
using Fingear.MonoGame;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;
using Diese.Collections;
using Glyph.Core.Inputs;

namespace Glyph.UI.Controls
{
    public abstract class ButtonBase : GlyphObject, IButton
    {
        private readonly ControlManager _controlManager;
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
                else if (!Hover)
                    Leaved?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Triggered;
        public event EventHandler Released;
        public event EventHandler Entered;
        public event EventHandler Leaved;

        protected ButtonBase(ControlManager controlManager, IDependencyInjector injector)
            : base(injector)
        {
            _controlManager = controlManager;

            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            Text = Add<Text>();

            Schedulers.Update.Plan(HandleInput);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            bool hover = Hover;
            bool isMouseUsed = _controlManager.InputSources.Any<MouseSource>();

            if (isMouseUsed)
            {
                if (_controlManager.Layers.Any(out CursorControls cursorControls) && cursorControls.VirtualScreenPosition.IsActive(out System.Numerics.Vector2 mousePosition))
                    hover = Frame.Bounds.ContainsPoint(mousePosition.AsMonoGameVector());
            }

            if (Hover)
            {
                if (isMouseUsed)
                {
                    if (_controlManager.Layers.Any(out CursorControls cursorControls))
                    {
                        if (cursorControls.Clic.IsActive())
                            Trigger();
                        else if (cursorControls.ReleaseClic.IsActive())
                            Release();
                    }
                }
                else if (_controlManager.Layers.Any(out MenuControls menuControls) && menuControls.Confirm.IsActive())
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
            Triggered?.Invoke(this, EventArgs.Empty);
        }

        private void Release()
        {
            Pressed = false;
            Released?.Invoke(this, EventArgs.Empty);
        }
    }
}