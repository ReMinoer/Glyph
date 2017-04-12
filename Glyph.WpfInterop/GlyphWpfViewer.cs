using System;
using System.Windows;
using Fingear.MonoGame;
using Glyph.Core;
using Glyph.Engine;
using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpfViewer : D3D11Client<GlyphWpfRunner>, IGlyphClient
    {
        private readonly WpfInputStates _wpfInputStates;
        public Resolution Resolution { get; }
        public Predicate<IView> IsViewVisiblePredicate { get; set; }
        GraphicsDevice IGlyphClient.GraphicsDevice => GraphicsDevice;
        IInputStates IInputClient.States => _wpfInputStates;

        public GlyphWpfViewer()
        {
            Focusable = true;
            SizeChanged += OnClientSizeChanged;

            _wpfInputStates = new WpfInputStates(this);
            Resolution = new Resolution();
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Runner == null)
                return;

            Runner.Engine?.BeginDraw();
            if (Runner.Engine == null)
                return;

            base.Draw(gameTime);

            var drawContext = new GlyphEngine.DrawContext(GraphicsDevice, Resolution)
            {
                DefaultRenderTarget = RenderTarget,
                IsViewVisiblePredicate = IsViewVisiblePredicate
            };
            Runner.Engine.Draw(drawContext);

            Runner.Engine.EndDraw();
        }

        private void OnClientSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resolution.WindowSize = new Vector2((int)e.NewSize.Width, (int)e.NewSize.Height);
        }
    }
}