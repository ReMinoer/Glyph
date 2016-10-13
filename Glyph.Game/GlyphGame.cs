using System;
using System.Globalization;
using System.Linq;
using Diese.Injection;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Input;
using Glyph.Input.StandardControls;
using Glyph.Tools;
using Glyph.Tools.StatusDisplay;
using Glyph.Tools.StatusDisplay.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;

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
        private Drawer _drawer;
        private bool _sceneChanged;
        private CultureInfo _culture;
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public ContentLibrary ContentLibrary { get; private set; }
        public ControlManager ControlManager { get; private set; }
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

            ControlManager = new ControlManager();

            Culture = CultureInfo.CurrentCulture;

            PerformanceViewer = new PerformanceViewer();
            StatusDisplay = new StatusDisplay();
            StatusDisplay.Channels.Add(new DefautStatusDisplayChannel(PerformanceViewer));

            Registry.RegisterInstance<GlyphGame>(this);
            Registry.RegisterInstance<ContentLibrary>(ContentLibrary);
            Registry.RegisterInstance<ControlManager>(ControlManager);
            Registry.RegisterLazy(() => SpriteBatch);
            Registry.RegisterLazy(() => GraphicsDevice);
        }

        protected override void Initialize()
        {
            ViewManager.Main.Initialize();
            Scene.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            _drawer = new Drawer(SpriteBatch, GraphicsDeviceManager);

            ContentLibrary.LoadContent(Content);

            ViewManager.Main.LoadContent(ContentLibrary);
            SongPlayer.Instance.LoadContent(ContentLibrary);
            StatusDisplay.LoadContent(ContentLibrary);

            Scene.LoadContent(ContentLibrary);
            _sceneChanged = false;
        }

        protected override void Update(GameTime gameTime)
        {
            ElapsedTime elapsedTime = ElapsedTime.Instance;
            elapsedTime.Refresh(gameTime);

            PerformanceViewer.UpdateCall();

            base.Update(gameTime);

            ControlManager.Update(elapsedTime, IsFocus);

            if (!IsFocus)
                return;

            HandleInput();
            using (GlyphSchedulableBase.UpdateWatchTree.Start("Root"))
            {
                do
                {
                    if (_sceneChanged)
                    {
                        Scene.Initialize();
                        Scene.LoadContent(ContentLibrary);

                        _sceneChanged = false;
                    }
                    Scene.Update(elapsedTime);
                } while (_sceneChanged);

                ViewManager.Main.Update(elapsedTime);
                SongPlayer.Instance.Update(elapsedTime);
                PerformanceViewer.Update(gameTime);
                StatusDisplay.Update(gameTime);

                PerformanceViewer.UpdateEnd();
            }

            if (GlyphSchedulableBase.UpdateWatchTree.Enabled)
            {
                string filename = "watchtree_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss");
                GlyphSchedulableBase.UpdateWatchTree.Results.First().Value.SaveToCsv(filename);

                GlyphSchedulableBase.UpdateWatchTree.Enabled = false;
            }
        }

        protected virtual void HandleInput()
        {
            DeveloperControls developerControls;
            if (ControlManager.TryGetLayer(out developerControls))
            {
                if (developerControls.Mute.IsActive())
                    MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

                if (developerControls.XboxQuit.IsActive())
                    Exit();

                if (developerControls.UpdateSnapshot.IsActive())
                    GlyphSchedulableBase.UpdateWatchTree.Enabled = true;

                if (developerControls.CompositionLog.IsActive())
                    CompositionLog.Write(Scene, Scene.RootNode);

#if WINDOWS
                if (developerControls.Fullscreen.IsActive())
                    Resolution.Instance.ToogleFullscreen();
#endif
                
                if (developerControls.ToogleSong.IsActive())
                    switch (MediaPlayer.State)
                    {
                        case MediaState.Playing:
                            MediaPlayer.Pause();
                            break;
                        case MediaState.Paused:
                            MediaPlayer.Resume();
                            break;
                    }

                if (developerControls.PreviousSong.IsActive())
                    SongPlayer.Instance.Previous();

                if (developerControls.NextSong.IsActive())
                    SongPlayer.Instance.Next();

                if (developerControls.StatusDisplay.IsActive())
                    StatusDisplay.Visible = !StatusDisplay.Visible;
            }
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

            foreach (IView view in ViewManager.Main.Views)
            {
                _drawer.CurrentView = view;
                view.PrepareDraw(_drawer);
                Scene.Draw(drawer);
            }
            
            drawer.SpriteBatchStack.Push(SpriteBatchContext.Default);
            StatusDisplay.Draw(drawer.SpriteBatchStack.Current);
            drawer.SpriteBatchStack.Pop();
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