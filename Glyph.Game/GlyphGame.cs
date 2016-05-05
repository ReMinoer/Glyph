using System;
using System.Globalization;
using System.Linq;
using Diese.Injection;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Effects;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Glyph.Scripting;
using Glyph.Tools;
using Glyph.Tools.StatusDisplay;
using Glyph.Tools.StatusDisplay.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;
using SongPlayer = Glyph.Tools.SongPlayer;

namespace Glyph.Game
{
    public abstract class GlyphGame : Microsoft.Xna.Framework.Game
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected IDependencyRegistry Registry;
        protected IDependencyInjector Injector;
        protected int DefaultWindowHeight;
        protected int DefaultWindowWidth;
        private IScene _scene;
        private bool _sceneChanged;
        private CultureInfo _culture;
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public ContentLibrary ContentLibrary { get; private set; }
        public InputManager InputManager { get; private set; }
        public ScriptManager ScriptManager { get; private set; }
        public PerformanceViewer PerformanceViewer { get; private set; }
        public StatusDisplay StatusDisplay { get; private set; }

        public IScene Scene
        {
            get { return _scene; }
            set
            {
                if (_scene == value)
                    return;

                _scene = value;
                _sceneChanged = true;
                Logger.Info("Change scene : " + _scene.Name);
            }
        }

        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
                Logger.Info("Current culture : " + _culture.EnglishName);
            }
        }

        public virtual bool IsFocus
        {
            get { return IsActive; }
        }

        private IDrawer _drawer;

        protected GlyphGame(string[] args, IDependencyRegistry dependencyRegistry)
        {
            Registry = dependencyRegistry;
            Injector = new RegistryInjector(Registry);
            Registry.RegisterInstance<IDependencyInjector>(Injector);

            Logger.Info("Start game");
            Logger.Info("Launch arguments : " + (args.Any() ? args.Aggregate((x, y) => x + " " + y) : ""));

            GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;

            Resolution.Instance.Init(GraphicsDeviceManager, Window);

            DefaultWindowHeight = (int)Resolution.Instance.WindowSize.Y;
            DefaultWindowWidth = (int)Resolution.Instance.WindowSize.X;

            GraphicsDeviceManager.PreferMultiSampling = true;
            GraphicsDeviceManager.ApplyChanges();
            IsMouseVisible = false;

            Content.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();

            InputManager = new InputManager();
            ScriptManager = new ScriptManager();
            Culture = CultureInfo.CurrentCulture;

            PerformanceViewer = new PerformanceViewer();
            StatusDisplay = new StatusDisplay();
            StatusDisplay.Channels.Add(new DefautStatusDisplayChannel(PerformanceViewer));

            EditorCursor.Initialize();
            Chronometer.Init();

            Registry.RegisterInstance<GlyphGame>(this);
            Registry.RegisterInstance<ContentLibrary>(ContentLibrary);
            Registry.RegisterInstance<InputManager>(InputManager);
            Registry.RegisterInstance<ScriptManager>(ScriptManager);
            Registry.RegisterLazy(() => SpriteBatch);
            Registry.RegisterLazy(() => GraphicsDevice);
        }

        protected override void Initialize()
        {
            ScreenEffectManager.Instance.Initialize();

            Scene.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _drawer = new Drawer(SpriteBatch, GraphicsDeviceManager);

            ContentLibrary.LoadContent(Content);

            ScreenEffectManager.Instance.LoadContent(ContentLibrary, GraphicsDevice);
            AudioManager.LoadContent(ContentLibrary);
            StatusDisplay.LoadContent(ContentLibrary);

            Scene.LoadContent(ContentLibrary);
            _sceneChanged = false;

#if WINDOWS
            EditorCursor.LoadContent(ContentLibrary);
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            ElapsedTime.Instance.Refresh(gameTime);
            PerformanceViewer.UpdateCall();

            base.Update(gameTime);

            InputManager.Update(IsFocus);

            if (!IsFocus)
                return;

            HandleInput();

            do
            {
                if (_sceneChanged)
                {
                    Scene.Initialize();
                    Scene.LoadContent(ContentLibrary);

                    _sceneChanged = false;
                }
                Scene.Update(ElapsedTime.Instance);
            } while (_sceneChanged);

            ScreenEffectManager.Instance.Update(gameTime);
            AudioManager.Update(gameTime);
            PerformanceViewer.Update(gameTime);
            StatusDisplay.Update(gameTime);

            PerformanceViewer.UpdateEnd();
        }

        protected virtual void HandleInput()
        {
            if (InputManager[DeveloperInputs.Mute])
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

            if (InputManager[DeveloperInputs.XboxQuit])
                Exit();

            if (InputManager[DeveloperInputs.CompositionLog])
                CompositionLog.Write(Scene, Scene.RootNode);

#if WINDOWS
            if (InputManager[DeveloperInputs.Fullscreen])
                Resolution.Instance.ToogleFullscreen();
#endif

            SongPlayer.HandleInput(InputManager);
            StatusDisplay.HandleInput(InputManager);

#if WINDOWS
            EditorCursor.HandleInput(InputManager);
#endif
        }

        protected override sealed void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Draw(_drawer);
        }

        protected virtual void Draw(IDrawer drawer)
        {
            if (!IsFocus)
                return;

            ScreenEffectManager.Instance.Prepare(drawer.SpriteBatchStack.Current, GraphicsDevice);
            ScreenEffectManager.Instance.CleanFirstRender(GraphicsDevice);

            Scene.PreDraw(drawer.SpriteBatchStack.Current);
            Scene.Draw(drawer);

            drawer.SpriteBatchStack.Push(new SpriteBatchContext());
            StatusDisplay.Draw(drawer.SpriteBatchStack.Current);
#if WINDOWS
            EditorCursor.Draw(drawer.SpriteBatchStack.Current);
#endif
            drawer.SpriteBatchStack.Pop();

            Scene.PostDraw();
        }

        protected override bool BeginDraw()
        {
            PerformanceViewer.DrawCall();

            Resolution.Instance.BeginDraw();
            GraphicsDevice.Clear(Color.Black);

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