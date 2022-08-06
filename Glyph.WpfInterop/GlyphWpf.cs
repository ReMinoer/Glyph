using System;
using System.Threading.Tasks;
using System.Windows;
using Fingear.MonoGame;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpf : WpfGame, IGlyphClient
    {
        private GlyphEngine _engine;
        private Task _loadContentTask;
        private readonly WpfInputStates _wpfInputStates;

        IInputStates IInputClient.States => _wpfInputStates;
        RenderTarget2D IDrawClient.DefaultRenderTarget => RenderTarget;

        private Vector2 GlyphSize => new Vector2((float)ActualWidth, (float)ActualHeight);
        Vector2 IDrawClient.Size => GlyphSize;

        private event Action<Vector2> GlyphSizeChanged;
        event Action<Vector2> IDrawClient.SizeChanged
        {
            add => GlyphSizeChanged += value;
            remove => GlyphSizeChanged -= value;
        }

        public GlyphEngine Engine
        {
            get { return _engine; }
            set
            {
                _engine = value;

                if (IsInitialized)
                {
                    Engine?.Initialize();
                    Task.Run(() => Engine?.LoadContentAsync()).Wait();
                }

                if (IsLoaded && Engine != null && !Engine.IsStarted)
                    _engine.Start();
            }
        }

        public GlyphWpf()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;

            _wpfInputStates = new WpfInputStates(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Engine?.Start();
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Engine?.Stop();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlyphSizeChanged?.Invoke(GlyphSize);
        }

        protected override void Initialize()
        {
            Engine?.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _loadContentTask = Engine.LoadContentAsync();
        }

        protected override void Update(GameTime gameTime)
        {
            Engine?.BeginUpdate(gameTime);

            base.Update(gameTime);

            // Do not update engine until loading is done.
            if (!_loadContentTask.IsCompleted)
                return;
            _loadContentTask.Wait();

            Engine?.HandleInput();

            if (!IsEnabled)
                return;

            Engine?.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            Engine?.BeginDraw();

            if (Engine == null)
                return;

            base.Draw(gameTime);

            // Do not draw engine until loading is done.
            if (_loadContentTask.IsCompleted)
            {
                _loadContentTask.Wait();
                Engine.Draw(this);
            }

            Engine.EndDraw();
        }
    }
}