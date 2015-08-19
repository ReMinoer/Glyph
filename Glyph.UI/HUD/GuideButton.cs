using System;
using Glyph.Entities;
using Glyph.Input;
using Glyph.Input.Handlers.Buttons;
using Glyph.Input.StandardInputs;
using Glyph.Transition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Glyph.UI.HUD
{
    public class GuideButton : Sprite
    {
        protected readonly string ActionButtonName;
        protected readonly TextSprite TextSprite = new TextSprite();
        protected readonly TransitionFloat TransitionOpacity = new TransitionFloat {Start = 0, End = 1, Duration = 500};
        protected bool IsPad;
        private readonly bool _clickable;
        private readonly Vector2 _keyPadding = new Vector2(20, 50);
        public virtual bool Display { get; set; }
        public bool DisplayKeyText { get; set; }
        public Texture2D KeyboardIcon { get; protected set; }
        public Texture2D PadIcon { get; protected set; }

        public override Texture2D Texture
        {
            get { return IsPad ? PadIcon : KeyboardIcon; }
        }

        public override float Opacity
        {
            get { return base.Opacity * TransitionOpacity; }
        }

        public string KeyName
        {
            get { return TextSprite.Text; }
        }

        public event EventHandler Activated;
        public event EventHandler IconChanged;

        public GuideButton(string actionButtonName, bool clickable = false)
        {
            ActionButtonName = actionButtonName;
            _clickable = clickable;
        }

        public override void Initialize()
        {
            base.Initialize();

            Display = true;
            DisplayKeyText = true;

            TextSprite.Initialize();
            TextSprite.Color = Color.Black;

            TransitionOpacity.Reset();
        }

        public void LoadContent(ContentLibrary content, string assetKb, string assetPad = "")
        {
            KeyboardIcon = content.GetTexture(assetKb);

            if (assetPad != "")
                PadIcon = content.GetTexture(assetPad);

            TextSprite.LoadContent(content, "guide-keys");

            if (IconChanged != null)
                IconChanged(this, EventArgs.Empty);
        }

        public override void Update(GameTime gameTime)
        {
            TransitionOpacity.Update(gameTime, !Display);
            TextSprite.Opacity = Opacity;

            if (Texture != null)
            {
                Vector2 ratio = (Texture.Size() - _keyPadding) / TextSprite.MeasureString();
                TextSprite.Scale = MathHelper.Min(ratio.X, ratio.Y) * Scale;
                TextSprite.Center = Center;
            }
        }

        public void GetKeyName(InputManager input)
        {
            IInputHandler inputHandler = input.Controls[ActionButtonName];
            if (inputHandler == null)
                return;

            KeyHandler keyHandler = inputHandler as KeyHandler ?? inputHandler.GetComponentInChildren<KeyHandler>();
            if (keyHandler == null)
                return;

            TextSprite.Text = Enum.GetName(typeof(Keys), keyHandler.Key);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!IsPad && DisplayKeyText)
                TextSprite.Draw(spriteBatch);
        }

        public virtual void HandleInput(InputManager input)
        {
            if (KeyName == "")
                GetKeyName(input);

            if (IsPad != input.IsGamePadUsed)
            {
                TransitionOpacity.Reset();
                IsPad = input.IsGamePadUsed;
                if (IconChanged != null)
                    IconChanged(this, EventArgs.Empty);
            }

            if (_clickable)
            {
                var mouseInScreen = input.GetValue<Vector2>(MouseInputs.VirtualScreenPosition);
                bool hover = Hitbox.Contains(mouseInScreen);
                if (hover && input[MenuInputs.Clic])
                    if (Activated != null)
                        Activated(this, EventArgs.Empty);
            }
        }
    }
}