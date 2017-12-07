using System;
using System.Windows;
using Fingear.MonoGame;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpfViewer : D3D11Client, IGlyphClient
    {
        private readonly WpfInputStates _wpfInputStates;
        public Resolution Resolution { get; }
        Matrix IDrawClient.ResolutionMatrix => Resolution.TransformationMatrix;
        RenderTarget2D IDrawClient.DefaultRenderTarget => RenderTarget;
        GraphicsDevice IDrawClient.GraphicsDevice => GraphicsDevice;
        IInputStates IInputClient.States => _wpfInputStates;

        public GlyphWpfViewer()
        {
            Focusable = true;
            SizeChanged += OnClientSizeChanged;

            _wpfInputStates = new WpfInputStates(this);
            Resolution = new Resolution();
        }

        private void OnClientSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resolution.WindowSize = new Vector2((int)e.NewSize.Width, (int)e.NewSize.Height);
        }
    }
}