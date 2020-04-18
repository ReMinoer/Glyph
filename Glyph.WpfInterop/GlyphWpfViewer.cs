using System;
using System.Windows;
using Fingear.MonoGame;
using Glyph.Core.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpfViewer : D3D11Client, IWpfGlyphClient
    {
        private readonly WpfInputStates _wpfInputStates;
        RenderTarget2D IDrawClient.DefaultRenderTarget => RenderTarget;
        GraphicsDevice IDrawClient.GraphicsDevice => GraphicsDevice;
        IInputStates IInputClient.States => _wpfInputStates;

        private Vector2 GlyphSize => new Vector2((float)ActualWidth, (float)ActualHeight);
        Vector2 IDrawClient.Size => GlyphSize;

        private event Action<Vector2> GlyphSizeChanged;
        event Action<Vector2> IDrawClient.SizeChanged
        {
            add => GlyphSizeChanged += value;
            remove => GlyphSizeChanged -= value;
        }

        public GlyphWpfViewer()
        {
            SizeChanged += OnSizeChanged;

            Focusable = true;
            _wpfInputStates = new WpfInputStates(this);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlyphSizeChanged?.Invoke(GlyphSize);
        }
    }
}