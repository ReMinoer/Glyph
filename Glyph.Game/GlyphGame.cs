using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Glyph.Audio;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Glyph.Tools;
using Glyph.Tools.StatusDisplayChannels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Game
{
    public abstract class GlyphGame<TMode> : Microsoft.Xna.Framework.Game
    {
        protected int DefautWindowHeight = (int)Resolution.Instance.WindowSize.Y;
        protected int DefautWindowWidth = (int)Resolution.Instance.WindowSize.X;
        private CultureInfo _culture;
        private Viewport _defaultViewport;
        private bool _synchroVertical;
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public ContentLibrary ContentLibrary { get; private set; }
        public InputManager InputManager { get; private set; }
        public ModeManager<TMode> ModeManager { get; private set; }
        public Dictionary<TMode, IGameMode> Modes { get; private set; }
        public PerformanceViewer PerformanceViewer { get; private set; }
        public StatusDisplay StatusDisplay { get; private set; }

        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
                Logger.Info("Current culture : " + _culture.EnglishName);
            }
        }

        public bool SynchroVertical
        {
            get { return _synchroVertical; }
            set
            {
                Graphics.SynchronizeWithVerticalRetrace = value;
                _synchroVertical = value;
            }
        }

        public virtual bool IsFocus
        {
            get { return IsActive; }
        }

        protected GlyphGame(string[] args)
        {
            Logger.Info("Start game");
            Logger.Info("Launch arguments : " + (args.Any() ? args.Aggregate((x, y) => x + " " + y) : ""));

            Graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;

            Resolution.Instance.Init(Graphics, Window);

            Graphics.PreferMultiSampling = true;
            Graphics.ApplyChanges();
            IsMouseVisible = false;

            Content.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();

            InputManager = InputManager.Instance;

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
            if (ElapsedTime.Instance == null)
                ElapsedTime.Instance = gameTime;

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

            if (InputManager[DeveloperInputs.Mute])
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

            if (InputManager[DeveloperInputs.XboxQuit])
                Exit();

#if WINDOWS
            if (InputManager[DeveloperInputs.Fullscreen])
                Resolution.Instance.ToogleFullscreen();
#endif

            Modes[ModeManager.State].HandleInput(InputManager);
            SongPlayer.HandleInput(InputManager);
            StatusDisplay.HandleInput(InputManager);

#if WINDOWS
            EditorCursor.HandleInput(InputManager);
#endif
        }

        protected override sealed void Draw(GameTime gameTime)
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

        protected virtual void Draw()
        {
        }

        protected override bool BeginDraw()
        {
            _defaultViewport = GraphicsDevice.Viewport;
            PerformanceViewer.DrawCall();

            Resolution.Instance.BeginDraw();
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
            Resolution.Instance.SetWindow(Window.ClientBounds.Width, Window.ClientBounds.Height,
                Resolution.Instance.FullScreen);
        }
    }
}