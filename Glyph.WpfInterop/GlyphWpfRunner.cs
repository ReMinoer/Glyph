using System;
using System.Diagnostics;
using System.Windows.Media;
using Glyph.Game;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpfRunner : IGameRunner, IDisposable
    {
        private GlyphEngine _engine;
        private readonly Stopwatch _timer = new Stopwatch();
        private TimeSpan _lastRenderingTime;
        private TimeSpan _timeSinceStart = TimeSpan.Zero;
        static private readonly TimeSpan MaxElapsedTime = TimeSpan.FromMilliseconds(500);

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
                _timer.Reset();

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

            _timer.Start();

            var renderingEventArgs = (RenderingEventArgs)e;
            if (_lastRenderingTime != renderingEventArgs.RenderingTime)
            {
                _lastRenderingTime = renderingEventArgs.RenderingTime;

                TimeSpan elapsed = _timer.Elapsed;
                TimeSpan diff = elapsed - _timeSinceStart;

                if (diff > MaxElapsedTime)
                {
                    elapsed -= diff - MaxElapsedTime;
                    diff = MaxElapsedTime;
                }

                _timeSinceStart = elapsed;

                var gameTime = new GameTime(_timer.Elapsed, diff);

                Engine?.BeginUpdate(gameTime);
                Engine?.HandleInput();
                Engine?.Update();

                Drawing?.Invoke(this, new DrawingEventArgs(gameTime));
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            if (_engine != null)
                CompositionTarget.Rendering -= OnRendering;

            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}