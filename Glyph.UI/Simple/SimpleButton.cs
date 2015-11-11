using System;
using Diese.Injection;
using Glyph.Input;
using Glyph.UI.Controls;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Simple
{
    public class SimpleButton : ButtonBase
    {
        public SimpleFrame Frame { get; private set; }
        public StatesColor FrameColors { get; set; }
        public StatesColor BorderColors { get; set; }
        public StatesColor TextColors { get; set; }

        protected override IFrame FrameBase
        {
            get { return Frame; }
        }

        public SimpleButton(InputManager inputManager, IDependencyInjector injector)
            : base(inputManager, injector)
        {
            Frame = Add<SimpleFrame>();

            FrameColors = new StatesColor
            {
                Released = Color.DarkGray,
                Hover = Color.Gray,
                Pressed = Color.LightGray,
            };

            BorderColors = new StatesColor(Color.Black);
            TextColors = new StatesColor(Color.Black);

            Frame.Color = FrameColors.Released;
            Text.SpriteTransformer.Color = TextColors.Released;

            Triggered += OnTriggered;
            Released += OnReleased;
            Entered += OnEntered;
            Leaved += OnLeaved;
        }

        private void OnTriggered(object sender, EventArgs eventArgs)
        {
            Frame.Color = FrameColors.Pressed;
            Frame.Border.Color = BorderColors.Pressed;
            Text.SpriteTransformer.Color = TextColors.Pressed;
        }

        private void OnReleased(object sender, EventArgs eventArgs)
        {
            Frame.Color = FrameColors.Hover;
            Frame.Border.Color = BorderColors.Hover;
            Text.SpriteTransformer.Color = TextColors.Hover;
        }

        private void OnEntered(object sender, EventArgs eventArgs)
        {
            Frame.Color = FrameColors.Hover;
            Frame.Border.Color = BorderColors.Hover;
            Text.SpriteTransformer.Color = TextColors.Hover;
        }

        private void OnLeaved(object sender, EventArgs eventArgs)
        {
            Frame.Color = FrameColors.Released;
            Frame.Border.Color = BorderColors.Released;
            Text.SpriteTransformer.Color = TextColors.Released;
        }

        public struct StatesColor
        {
            public Color Released { get; set; }
            public Color Hover { get; set; }
            public Color Pressed { get; set; }

            public StatesColor(Color singleColor)
                : this()
            {
                Released = singleColor;
                Hover = singleColor;
                Pressed = singleColor;
            }
        }
    }
}