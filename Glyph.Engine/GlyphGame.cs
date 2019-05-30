using System;
using Niddle;
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Interactives;
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

        private IControl _fullscreenControl;
        private Point _windowSize;
        private Point _lastWindowSize;
        private bool _resizing;

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
            
            _fullscreenControl = new ControlSimultaneous<IControl>
            {
                Components =
                {
                    new Control(InputSystem.Instance.Keyboard[Keys.LeftAlt], InputActivity.Pressed),
                    new Control(InputSystem.Instance.Keyboard[Keys.Enter])
                }
            };

            Engine.InteractionManager.Root.Add(new Interactive
            {
                Name = "Window interactive",
                Controls =
                {
                    _fullscreenControl
                }
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

            if (_fullscreenControl.IsActive)
                ToggleFullscreen();

            Engine.HandleInput();

            if (!IsActive)
                return;

            Engine.Update();
        }

        public void ToggleFullscreen()
        {
            if (_resizing)
                return;

            _resizing = true;

            if (Window.IsBorderless)
            {
                WindowSize = _lastWindowSize;
                Window.IsBorderless = false;
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