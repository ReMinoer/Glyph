﻿using System;
using System.Diagnostics;
using System.Windows.Media;
using Glyph.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpfRunner : IGameRunner
    {
        private GlyphEngine _engine;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly GameTime _gameTime = new GameTime();
        private TimeSpan _previousTime = TimeSpan.Zero;
        private TimeSpan _lastRenderingTime;
        public TimeSpan MaxElapsedTime { get; set; } = TimeSpan.FromSeconds(1.0 / 30);

        public GlyphEngine Engine
        {
            get => _engine;
            set
            {
                if (_engine == value)
                    return;

                if (_engine != null)
                    CompositionTarget.Rendering -= OnRendering;

                _engine = value;

                if (_engine != null)
                    CompositionTarget.Rendering += OnRendering;

                _stopwatch.Reset();
            }
        }


        public event EventHandler<DrawingEventArgs> Drawing;
        public event EventHandler Disposed;

        private void OnRendering(object sender, EventArgs e)
        {
            if (Engine == null || !Engine.IsInitialized || !Engine.IsLoaded || !Engine.IsStarted)
                return;

            _stopwatch.Start();

            var renderingEventArgs = (RenderingEventArgs)e;
            if (_lastRenderingTime == renderingEventArgs.RenderingTime)
                return;

            _lastRenderingTime = renderingEventArgs.RenderingTime;

            TimeSpan currentTime = _stopwatch.Elapsed;
            TimeSpan delta = currentTime - _previousTime;
            _previousTime = currentTime;

            if (delta > MaxElapsedTime)
                delta = MaxElapsedTime;

            _gameTime.ElapsedGameTime = delta;
            _gameTime.TotalGameTime += delta;

            Engine.BeginUpdate(_gameTime);
            Engine.HandleInput();
            Engine.Update();

            Drawing?.Invoke(this, new DrawingEventArgs(_gameTime));
        }

        public void Draw(D3D11Client client, GameTime gameTime)
        {
            Engine.BeginDraw();
            Engine.Draw(client as IDrawClient ?? new DrawClient(client));
            Engine.EndDraw();
        }

        public class DrawClient : IDrawClient
        {
            private readonly D3D11Client _client;
            public Vector2 Size { get; }
            public GraphicsDevice GraphicsDevice => D3D11Client.GraphicsDevice;
            public RenderTarget2D DefaultRenderTarget => _client.RenderTarget;
            public event Action<Vector2> SizeChanged;

            public DrawClient(D3D11Client client)
            {
                _client = client;
                Size = new Vector2(DefaultRenderTarget.Width, DefaultRenderTarget.Height);
            }
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            if (Engine != null)
            {
                CompositionTarget.Rendering -= OnRendering;
                Engine = null;
            }

            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}