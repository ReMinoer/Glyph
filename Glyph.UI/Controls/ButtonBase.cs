using System;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Animation;
using Glyph.Core;
using Glyph.Core.Inputs;

namespace Glyph.UI.Controls
{
    public abstract class ButtonBase : GlyphObject, IButton
    {
        private bool _hover;
        private readonly SceneCursorControl _sceneCursor;
        private readonly ActivityControl _clic;
        private readonly IControl<InputActivity> _confirm;
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Text Text { get; private set; }
        public bool Pressed { get; private set; }
        public abstract IFrame Frame { get; }

        public bool Hover
        {
            get => _hover;
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

        protected ButtonBase(GlyphInjectionContext context, InputClientManager inputClientManager, ViewManager viewManager)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            Text = Add<Text>();

            var controls = Add<Core.Inputs.Controls>();
            controls.Tags.Add(ControlLayerTag.Ui);
            controls.RegisterMany(new Fingear.IControl[]
            {
                _sceneCursor = new SceneCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, inputClientManager, viewManager),
                _clic = new ActivityControl("Clic", InputSystem.Instance.Mouse[MouseButton.Left]),
                _confirm = MenuControls.Instance.Confirm
            });

            Schedulers.Update.Plan(HandleInput);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            bool hover = Hover;
            bool isMouseUsed = InputManager.Instance.InputSources.AnyOfType<MouseSource>();

            if (isMouseUsed)
            {
                if (_sceneCursor.IsActive(out System.Numerics.Vector2 mousePosition))
                    hover = Frame.Bounds.ContainsPoint(mousePosition.AsMonoGameVector());
            }

            if (hover)
            {
                if (isMouseUsed)
                {
                    if (_clic.IsActive(out InputActivity inputActivity))
                    {
                        switch (inputActivity)
                        {
                            case InputActivity.Triggered:
                                Trigger();
                                break;
                            case InputActivity.Released:
                                Release();
                                break;
                        }
                    }
                }
                else if (_confirm.IsActive())
                {
                    Trigger();
                    Release();
                }
            }
            else if (Pressed)
                Pressed = false;

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