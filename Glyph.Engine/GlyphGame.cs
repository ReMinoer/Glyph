using System;
using Diese.Collections;
using Niddle;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Glyph.Core.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IInputStates = Fingear.MonoGame.IInputStates;

namespace Glyph.Engine
{
    public class GlyphGame : Game, IGlyphClient
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Point _windowSize;
        private Point _lastWindowSize;
        private bool _resizing;
        private readonly IControl _toggleFullscreen;
        public GlyphEngine Engine { get; }
        public virtual bool IsFocus => IsActive && System.Windows.Forms.Form.ActiveForm?.Handle == Window.Handle;
        IInputStates IInputClient.States { get; } = new InputStates();
        Vector2 IDrawClient.Size => WindowSize.ToVector2();
        RenderTarget2D IDrawClient.DefaultRenderTarget => null;
        GraphicsDevice IDrawClient.GraphicsDevice => GraphicsDevice;

        public event Action<Vector2> SizeChanged;

        public Point WindowSize
        {
            get => _windowSize;
            set
            {
                if (_graphicsDeviceManager.IsFullScreen)
                {
                    _lastWindowSize = value;
                    return;
                }

                if (_windowSize == value)
                    return;

                _windowSize = value;

                _graphicsDeviceManager.PreferredBackBufferWidth = value.X;
                _graphicsDeviceManager.PreferredBackBufferHeight = value.Y;
                _graphicsDeviceManager.ApplyChanges();

                SizeChanged?.Invoke(_windowSize.ToVector2());
            }
        }

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
            
            _windowSize = new Point(_graphicsDeviceManager.PreferredBackBufferWidth, _graphicsDeviceManager.PreferredBackBufferHeight);
            _lastWindowSize = WindowSize;

            Engine = new GlyphEngine(Content, dependencyConfigurator);
            Engine.Stopped += OnEngineStopped;
            Engine.FocusedClient = this;

            Engine.ControlManager.Plan(new ControlLayer("Window controls", ControlLayerTag.Debug)).RegisterMany(new []
            {
                _toggleFullscreen = new Control("Toogle fullscreen", InputSystem.Instance.Keyboard[Keys.F12])
            });
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
            
            if (_toggleFullscreen.IsActive())
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
                WindowSize = _lastWindowSize;
            }
            else
            {
                _lastWindowSize = WindowSize;

                int maxWidth = 0;
                int maxHeight = 0;
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    if (dm.Width > maxWidth || dm.Width == maxWidth && dm.Height > maxHeight)
                    {
                        maxWidth = dm.Width;
                        maxHeight = dm.Height;
                    }

                _graphicsDeviceManager.IsFullScreen = true;
                Window.IsBorderless = true;
                Window.Position = new Point(0, 0);
                WindowSize = new Point(maxWidth, maxHeight);
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
            Engine.Draw(this);
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
            WindowSize = Window.ClientBounds.Size;
            _resizing = false;
        }
    }
}