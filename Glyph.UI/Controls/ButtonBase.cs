using System;
using Fingear.Controls;
using Glyph.Animation;
using Glyph.Core;
using Glyph.Core.Inputs;

namespace Glyph.UI.Controls
{
    public abstract class ButtonBase : GlyphObject, IButton
    {
        public SceneNode SceneNode { get; }
        public Motion Motion { get; }
        public Text Text { get; }
        public bool Pressed { get; private set; }

        public abstract IFrame Frame { get; }
        
        private bool _hover;
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

        protected ButtonBase(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            Text = Add<Text>();

            var userInterface = Add<UserInterface>();
            userInterface.CursorMoved += OnCursorMoved;
            userInterface.TouchStarted += OnTouchStarted;
            userInterface.TouchEnded += OnTouchEnded;
            userInterface.Confirmed += OnConfirmed;
        }

        private void OnCursorMoved(object sender, CursorEventArgs e)
        {
            Hover = Frame.Bounds.ContainsPoint(e.CursorPosition);
        }

        private void OnTouchStarted(object sender, HandlableTouchEventArgs e)
        {
            if (!Hover)
                return;

            e.Handle();
            Trigger();
        }

        private void OnTouchEnded(object sender, CursorEventArgs e)
        {
            if (Hover)
                Release();
            else
                Pressed = false;
        }

        private void OnConfirmed(object sender, HandlableEventArgs e)
        {
            if (!Hover)
                return;

            e.Handle();
            Trigger();
            Release();
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