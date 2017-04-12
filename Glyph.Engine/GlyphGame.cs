using System;
using System.Windows.Forms;
using Diese.Injection;
using Fingear.MonoGame;
using Glyph.Input;
using Glyph.Input.StandardControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Engine
{
    public class GlyphGame : Game, IGlyphClient, IInputClient
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly GameInputStates _gameInputStates;
        private Vector2 _lastWindowSize;
        private bool _resizing;
        public GlyphEngine Engine { get; }
        public Resolution Resolution { get; }
        public virtual bool IsFocus => IsActive && Form.ActiveForm?.Handle == Window.Handle;
        IInputStates IInputClient.States => _gameInputStates;
        GraphicsDevice IGlyphClient.GraphicsDevice => GraphicsDevice;

        public GlyphGame(Action<IDependencyRegistry> dependencyConfigurator = null)
        {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
            IsMouseVisible = false;

            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true
            };
            _graphicsDeviceManager.ApplyChanges();

            _gameInputStates = new GameInputStates();

            Resolution = new Resolution();
            _lastWindowSize = Resolution.WindowSize;

            Engine = new GlyphEngine(Content, dependencyConfigurator);
            Engine.Stopped += OnEngineStopped;
            Engine.FocusedClient = this;
        }

        protected override void Initialize()
        {
            Engine.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Engine.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Engine.BeginUpdate(gameTime);

            base.Update(gameTime);

            Engine.HandleInput();

            if (!IsActive)
                return;

            DeveloperControls developerControls;
            if (Engine.ControlManager.TryGetLayer(out developerControls) && developerControls.Fullscreen.IsActive())
                ToggleFullscreen();

            Engine.Update();
        }

        public void ToggleFullscreen()
        {
            if (_resizing)
                return;

            _resizing = true;

            if (_graphicsDeviceManager.IsFullScreen)
            {
                _graphicsDeviceManager.IsFullScreen = false;
                Window.IsBorderless = false;
                _graphicsDeviceManager.PreferredBackBufferWidth = (int)_lastWindowSize.X;
                _graphicsDeviceManager.PreferredBackBufferHeight = (int)_lastWindowSize.Y;
                _graphicsDeviceManager.ApplyChanges();
                Resolution.WindowSize = _lastWindowSize;
            }
            else
            {
                _lastWindowSize = Resolution.WindowSize;

                int maxWidth = 0;
                int maxHeight = 0;
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    if (dm.Width >= maxWidth && dm.Height >= maxHeight
                        && dm.Width <= VirtualResolution.Size.X && dm.Height <= VirtualResolution.Size.Y)
                    {
                        maxWidth = dm.Width;
                        maxHeight = dm.Height;
                    }

                _graphicsDeviceManager.IsFullScreen = true;
                Window.IsBorderless = true;
                Window.Position = new Point(0, 0);
                _graphicsDeviceManager.PreferredBackBufferWidth = maxWidth;
                _graphicsDeviceManager.PreferredBackBufferHeight = maxHeight;
                _graphicsDeviceManager.ApplyChanges();
                Resolution.WindowSize = new Vector2(maxWidth, maxHeight);
            }

            _resizing = false;
        }

        protected override bool BeginDraw()
        {
            Engine.BeginDraw();
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Engine.Draw(new GlyphEngine.DrawContext(GraphicsDevice, Resolution));
        }

        protected override void EndDraw()
        {
            base.EndDraw();
            Engine.EndDraw();
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            Engine.Start();
        }

        protected override void EndRun()
        {
            Engine.Stop();
            base.EndRun();
        }

        public void OnEngineStopped()
        {
            Exit();
        }

        private void OnClientSizeChanged(object sender, EventArgs args)
        {
            if (_resizing)
                return;

            _resizing = true;

            _graphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Size.X;
            _graphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Size.Y;
            _graphicsDeviceManager.ApplyChanges();
            Resolution.WindowSize = Window.ClientBounds.Size.ToVector2();

            _resizing = false;
        }
    }
}