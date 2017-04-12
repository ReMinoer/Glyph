using System;
using System.Diagnostics;
using System.Windows.Media;
using Glyph.Engine;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpfRunner : IGameRunner, IDisposable
    {
        private GlyphEngine _engine;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly GameTime _gameTime = new GameTime();
        private TimeSpan _previousTime = TimeSpan.Zero;
        private TimeSpan _lastRenderingTime;
        public TimeSpan MaxElapsedTime { get; set; } = TimeSpan.FromSeconds(1.0 / 30);

        public GlyphEngine Engine
        {
            get { return _engine; }
            set
            {
                if (_engine == value)
                    return;

                if (_engine != null)
                    CompositionTarget.Rendering -= OnRendering;

                _engine = value;

                if (_engine == null)
                    return;
                
                CompositionTarget.Rendering += OnRendering;
                _stopwatch.Reset();

                if (!Engine.IsInitialized)
                {
                    Engine?.Initialize();
                    Engine?.LoadContent();
                }

                if (Engine.IsLoaded && !Engine.IsStarted)
                    _engine.Start();
            }
        }

        public event EventHandler<DrawingEventArgs> Drawing;
        public event EventHandler Disposed;

        private void OnRendering(object sender, EventArgs e)
        {
            if (Engine == null || !Engine.IsInitialized || !Engine.IsLoaded)
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

            Engine?.BeginUpdate(_gameTime);
            Engine?.HandleInput();
            Engine?.Update();

            Drawing?.Invoke(this, new DrawingEventArgs(_gameTime));
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            if (_engine != null)
                CompositionTarget.Rendering -= OnRendering;

            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}