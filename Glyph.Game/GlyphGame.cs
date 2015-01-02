using System;
using Glyph.Audio;
using Glyph.Input;
using Glyph.Input.StandardActions;
using Glyph.Tools;
using Glyph.Tools.StatusDisplayChannels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Glyph.Game
{
    public abstract class GlyphGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public ContentLibrary ContentLibrary { get; private set; }
        public InputManager InputManager { get; private set; }

        public PerformanceViewer PerformanceViewer { get; private set; }
        public StatusDisplay StatusDisplay { get; private set; }

        public bool SynchroVertical
        {
            get { return _synchroVertical; }
            set
            {
                Graphics.SynchronizeWithVerticalRetrace = value;
                _synchroVertical = value;
            }
        }

        public virtual bool IsFocus { get { return IsActive; } }
        protected int DefautWindowHeight = (int)Resolution.WindowSize.Y;
        protected int DefautWindowWidth = (int)Resolution.WindowSize.X;
        private Viewport _defaultViewport;
        private bool _synchroVertical;

        protected GlyphGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;

            Resolution.Init(Graphics, Window);

            Graphics.PreferMultiSampling = true;
            Graphics.ApplyChanges();
            IsMouseVisible = false;

            Content.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();

            InputManager = new InputManager(PlayerIndex.One);

            PerformanceViewer = new PerformanceViewer();
            StatusDisplay = new StatusDisplay();
            StatusDisplay.Channels.Add(new DefautStatusDisplayChannel(PerformanceViewer));
            EditorCursor.Initialize();

            Chronometer.Init();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ContentLibrary.LoadContent(Content, Graphics.GraphicsDevice);

            AudioManager.LoadContent(ContentLibrary);
            StatusDisplay.LoadContent(ContentLibrary);

#if WINDOWS
            EditorCursor.LoadContent(ContentLibrary);
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            PerformanceViewer.UpdateCall();

            base.Update(gameTime);
            HandleInput();

            AudioManager.Update(gameTime);
            PerformanceViewer.Update(gameTime);
            StatusDisplay.Update(gameTime);

            PerformanceViewer.UpdateEnd();
        }

        protected virtual void HandleInput()
        {
            InputManager.Update(IsFocus);
            if (!IsFocus)
                return;

            if (InputManager.IsActionDownNow(DeveloperActions.Mute))
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

            if (InputManager.IsActionDownNow(DeveloperActions.XboxQuit))
                Exit();

#if WINDOWS
            if (InputManager.IsActionDownNow(DeveloperActions.Fullscreen))
                Resolution.ToogleFullscreen();
#endif

            SongPlayer.HandleInput(InputManager);
            StatusDisplay.HandleInput(InputManager);

#if WINDOWS
            EditorCursor.HandleInput(InputManager);
#endif
        }

        protected sealed override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Draw();

            GraphicsDevice.Viewport = _defaultViewport;

            SpriteBatch.Begin();
            StatusDisplay.Draw(SpriteBatch);
#if WINDOWS
            EditorCursor.Draw(SpriteBatch);
#endif
            SpriteBatch.End();
        }

        protected virtual void Draw() {}

        protected override bool BeginDraw()
        {
            _defaultViewport = GraphicsDevice.Viewport;
            PerformanceViewer.DrawCall();

            Resolution.BeginDraw();
            Graphics.GraphicsDevice.Clear(Color.Black);

            return base.BeginDraw();
        }

        protected override void EndDraw()
        {
            base.EndDraw();
            PerformanceViewer.DrawEnd();
        }

        private void WindowSizeChanged(object sender, EventArgs e)
        {
            Resolution.SetWindow(Window.ClientBounds.Width, Window.ClientBounds.Height, Resolution.FullScreen);
        }
    }
}