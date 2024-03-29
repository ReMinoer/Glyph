using System;
using System.Threading.Tasks;
using Niddle;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Inputs;
using Fingear.Interactives;
using Fingear.MonoGame;
using Glyph.Core.Inputs;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IInputStates = Fingear.MonoGame.IInputStates;

namespace Glyph.Engine
{
    public class GlyphGame : Game, IGlyphClient
    {
        private Task _loadContentTask;
        private IControl _fullscreenControl;
        private Point _windowSize;
        private Point _lastWindowSize;
        private bool _resizing;

        public GlyphEngine Engine { get; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }

        // TODO: Make it cross-platform
        //[DllImport("user32.dll")]
        //static extern IntPtr GetForegroundWindow();
        //public virtual bool IsFocus => IsActive && GetForegroundWindow() == Window.Handle;

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
                if (GraphicsDeviceManager.IsFullScreen)
                {
                    _lastWindowSize = value;
                    return;
                }

                if (_windowSize == value)
                    return;

                _windowSize = value;

                GraphicsDeviceManager.PreferredBackBufferWidth = value.X;
                GraphicsDeviceManager.PreferredBackBufferHeight = value.Y;
                GraphicsDeviceManager.ApplyChanges();

                SizeChanged?.Invoke(_windowSize.ToVector2());
            }
        }

        public GlyphGame(ILogger logger, Func<IGraphicsDeviceService, IContentLibrary> contentLibraryCreator, Action<IDependencyRegistry> dependencyConfigurator = null)
        {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
            IsMouseVisible = false;

            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.ApplyChanges();
            
            _windowSize = new Point(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight);
            _lastWindowSize = WindowSize;

            Engine = new GlyphEngine(GraphicsDeviceManager, contentLibraryCreator(GraphicsDeviceManager), logger, dependencyConfigurator);
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
            _loadContentTask = Engine.LoadContentAsync();
        }

        protected override void Update(GameTime gameTime)
        {
            Engine.BeginUpdate(gameTime);

            base.Update(gameTime);

            if (_fullscreenControl.IsActive)
                ToggleFullscreen();

            // Do not update engine until loading is done.
            if (!_loadContentTask.IsCompleted)
                return;
            _loadContentTask.Wait();

            Engine.HandleInput();

            //if (!IsActive)
            //    return;

            Engine.Update();

            if (Engine.CursorManager.ChangeRequested)
            {
                if (Engine.CursorManager.Cursor != null)
                {
                    Mouse.SetCursor(Engine.CursorManager.Cursor);
                    IsMouseVisible = true;
                }
                else
                {
                    IsMouseVisible = false;
                }

                Engine.CursorManager.ResetRequest();
            }
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

            // Do not draw engine until loading is done.
            if (!_loadContentTask.IsCompleted)
                return;
            _loadContentTask.Wait();

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