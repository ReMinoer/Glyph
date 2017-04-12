using System;
using System.Windows;
using Fingear.MonoGame;
using Glyph.Core;
using Glyph.Engine;
using Glyph.Input;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public class GlyphWpf : WpfGame, IGlyphClient
    {
        private GlyphEngine _engine;
        private readonly WpfInputStates _wpfInputStates;
        public Resolution Resolution { get; }
        public Predicate<IView> IsViewVisiblePredicate { get; set; }
        IInputStates IInputClient.States => _wpfInputStates;
        public virtual bool IsFocus => IsFocused;

        public GlyphEngine Engine
        {
            get { return _engine; }
            set
            {
                _engine = value;

                if (IsInitialized)
                {
                    Engine?.Initialize();
                    Engine?.LoadContent();
                }

                if (IsLoaded && Engine != null && !Engine.IsStarted)
                    _engine.Start();
            }
        }

        public GlyphWpf()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnClientSizeChanged;

            _wpfInputStates = new WpfInputStates(this);
            Resolution = new Resolution();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Engine?.Start();
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Engine?.Stop();
        }

        protected override void Initialize()
        {
            Engine?.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Engine?.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Engine?.BeginUpdate(gameTime);

            base.Update(gameTime);

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

            var drawContext = new GlyphEngine.DrawContext(GraphicsDevice, Resolution)
            {
                DefaultRenderTarget = RenderTarget,
                IsViewVisiblePredicate = IsViewVisiblePredicate
            };
            Engine.Draw(drawContext);

            Engine.EndDraw();
        }

        private void OnClientSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resolution.WindowSize = new Vector2((int)e.NewSize.Width, (int)e.NewSize.Height);
        }
    }
}