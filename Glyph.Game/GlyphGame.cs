using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Glyph.Application;
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
    public abstract class GlyphGame<TMode> : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public ContentLibrary ContentLibrary { get; private set; }
        public InputManager InputManager { get; private set; }

        public ModeManager<TMode> ModeManager { get; private set; }
        public Dictionary<TMode, IGameMode> Modes { get; private set; }

        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
                Log.System(string.Format("Current culture : {0}", _culture.EnglishName));
            }
        }

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
        private CultureInfo _culture;

        protected GlyphGame(string[] args)
        {
            Log.StartStopwatch();
            Log.System("Start game");
            Log.System(string.Format("Launch arguments : {0}", args.Aggregate((x, y) => x + " " + y)));

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

            ModeManager = new ModeManager<TMode>();
            Modes = new Dictionary<TMode, IGameMode>();

            Culture = CultureInfo.CurrentCulture;

            PerformanceViewer = new PerformanceViewer();
            StatusDisplay = new StatusDisplay();
            StatusDisplay.Channels.Add(new DefautStatusDisplayChannel(PerformanceViewer));
            EditorCursor.Initialize();

            Chronometer.Init();
        }

        protected override void Initialize()
        {
            Modes[ModeManager.State].Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ContentLibrary.LoadContent(Content, Graphics.GraphicsDevice);

            AudioManager.LoadContent(ContentLibrary);
            StatusDisplay.LoadContent(ContentLibrary);

            Modes[ModeManager.State].LoadContent(ContentLibrary, GraphicsDevice);

#if WINDOWS
            EditorCursor.LoadContent(ContentLibrary);
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            PerformanceViewer.UpdateCall();

            base.Update(gameTime);

            bool change = ModeManager.HasChange();

            if (!change)
                HandleInput();

            AudioManager.Update(gameTime);
            PerformanceViewer.Update(gameTime);
            StatusDisplay.Update(gameTime);

            change = change || ModeManager.HasChange();
            do
            {
                if (change)
                {
                    Modes[ModeManager.State].Initialize();
                    Modes[ModeManager.State].LoadContent(ContentLibrary, GraphicsDevice);
                }
                Modes[ModeManager.State].Update(gameTime);
                change = ModeManager.HasChange();
            } while (change);

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

            Modes[ModeManager.State].HandleInput(InputManager);
            SongPlayer.HandleInput(InputManager);
            StatusDisplay.HandleInput(InputManager);

#if WINDOWS
            EditorCursor.HandleInput(InputManager);
#endif
        }

        protected sealed override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Modes[ModeManager.State].Draw(SpriteBatch, Graphics);

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