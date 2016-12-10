using System;
using System.Linq;
using Diese.Injection;
using Diese.Modelization;
using Glyph.Application;
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Game
{
    public class GlyphEngine
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ContentManager _contentManager;
        private IScene _scene;
        private bool _sceneChanged;
        public bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public IDependencyRegistry Registry { get; }
        public IDependencyInjector Injector { get; }
        public ContentLibrary ContentLibrary { get; }
        public ControlManager ControlManager { get; }
        public PerformanceViewer PerformanceViewer { get; }
        public StatusDisplay StatusDisplay { get; }

        public event Action Started;
        public event Action Stopped;
        public event Action Paused;

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

        public GlyphEngine(ContentManager contentManager, GraphicsDeviceManager mainGraphicsDevice, IConfigurator<IDependencyRegistry> dependencyConfigurator = null, params string[] args)
        {
            Logger.Info("Engine arguments : " + string.Join(" ", args));

            _contentManager = contentManager;
            
            IDependencyRegistry dependencyRegistry = new GlyphRegistry();
            dependencyConfigurator?.Configure(dependencyRegistry);

            Registry = dependencyRegistry;
            Injector = new RegistryInjector(Registry);
            Registry.RegisterInstance<IDependencyInjector>(Injector);

            _contentManager.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();

            ControlManager = new ControlManager();

            PerformanceViewer = new PerformanceViewer();
            StatusDisplay = new StatusDisplay();
            StatusDisplay.Channels.Add(new DefautStatusDisplayChannel(PerformanceViewer));

            Registry.RegisterInstance<GlyphEngine>(this);
            Registry.RegisterInstance<ContentLibrary>(ContentLibrary);
            Registry.RegisterInstance<ControlManager>(ControlManager);
            Registry.RegisterLazy(() => mainGraphicsDevice.GraphicsDevice);
        }

        public void Initialize()
        {
            ViewManager.Main.Initialize();
            Scene.Initialize();
        }

        public void LoadContent()
        {
            ContentLibrary.LoadContent(_contentManager);

            ViewManager.Main.LoadContent(ContentLibrary);
            SongPlayer.Instance.LoadContent(ContentLibrary);
            StatusDisplay.LoadContent(ContentLibrary);

            Scene.LoadContent(ContentLibrary);
            _sceneChanged = false;
        }

        public void BeginUpdate(GameTime gameTime)
        {
            ElapsedTime.Instance.Refresh(gameTime);
            PerformanceViewer.UpdateCall();
        }

        public void HandleInput(bool isFocus)
        {
            ControlManager.Update(ElapsedTime.Instance, isFocus);
        }

        public void Update()
        {
            ElapsedTime elapsedTime = ElapsedTime.Instance;

            DeveloperControls developerControls;
            if (ControlManager.TryGetLayer(out developerControls))
            {
                if (developerControls.Mute.IsActive())
                    MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

                if (developerControls.XboxQuit.IsActive())
                    Stop();

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
                PerformanceViewer.Update(elapsedTime.GameTime);
                StatusDisplay.Update(elapsedTime.GameTime);

                PerformanceViewer.UpdateEnd();
            }

            if (GlyphSchedulableBase.UpdateWatchTree.Enabled)
            {
                GlyphSchedulableBase.UpdateWatchTree.Results.First().Value.SaveToCsv("watchtree_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss"));
                GlyphSchedulableBase.UpdateWatchTree.Enabled = false;
            }
        }

        public void BeginDraw()
        {
            PerformanceViewer.DrawCall();
        }

        public void Draw(GraphicsDeviceManager graphicsDeviceManager)
        {
            var drawer = new Drawer(graphicsDeviceManager);

            Resolution.Instance.BeginDraw();
            graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            foreach (IView view in ViewManager.Main.Views)
            {
                drawer.CurrentView = view;
                view.PrepareDraw(drawer);
                Scene.Draw(drawer);
            }

            drawer.SpriteBatchStack.Push(SpriteBatchContext.Default);
            StatusDisplay.Draw(drawer.SpriteBatchStack.Current);
            drawer.SpriteBatchStack.Pop();
        }

        public void EndDraw()
        {
            PerformanceViewer.DrawEnd();
        }

        public void Start()
        {
            IsStarted = true;
            IsPaused = false;
            Logger.Info("Start engine");

            Started?.Invoke();
        }

        public void Stop()
        {
            IsStarted = false;
            Logger.Info("Stop engine");

            Stopped?.Invoke();
        }

        public void Pause()
        {
            IsPaused = true;
            Logger.Info("Pause engine");

            Paused?.Invoke();
        }
    }
}