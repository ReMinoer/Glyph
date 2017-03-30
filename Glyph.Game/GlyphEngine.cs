using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Diese.Injection;
using Glyph.Application;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Annotations;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Input;
using Glyph.Input.StandardControls;
using Glyph.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Game
{
    public class GlyphEngine : INotifyPropertyChanged
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ContentManager _contentManager;
        private readonly ElapsedTime _elapsedTime = new ElapsedTime();
        private IScene _scene;
        private bool _sceneChanged;
        private IGlyphClient _focusedClient;
        public bool IsInitialized { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public IDependencyRegistry Registry { get; }
        public IDependencyInjector Injector { get; }
        public ContentLibrary ContentLibrary { get; }
        public ControlManager ControlManager { get; }

        public IGlyphClient FocusedClient
        {
            get { return _focusedClient; }
            set
            {
                if (_focusedClient == value)
                    return;

                _focusedClient = value;
                ControlManager.InputClient = value;
                FocusChanged?.Invoke(_focusedClient);
            }
        }

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
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action Started;
        public event Action Stopped;
        public event Action Paused;
        public event Action<IGlyphClient> FocusChanged;

        public GlyphEngine(ContentManager contentManager, Action<IDependencyRegistry> dependencyConfigurator = null, params string[] args)
        {
            Logger.Info("Engine arguments : " + string.Join(" ", args));

            _contentManager = contentManager;
            
            IDependencyRegistry dependencyRegistry = new GlyphRegistry();
            dependencyConfigurator?.Invoke(dependencyRegistry);

            Registry = dependencyRegistry;
            Injector = new RegistryInjector(Registry);
            Registry.RegisterInstance<IDependencyInjector>(Injector);

            _contentManager.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();
            ControlManager = new ControlManager();

            Registry.RegisterInstance<GlyphEngine>(this);
            Registry.RegisterInstance<ContentLibrary>(ContentLibrary);
            Registry.RegisterInstance<ControlManager>(ControlManager);
            Registry.RegisterFunc(() =>
            {
                if (FocusedClient != null)
                {
                    return FocusedClient.GraphicsDevice;
                }
                
                return ((IGraphicsDeviceService)contentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            });
        }

        public void Initialize()
        {
            ViewManager.Main.Initialize();
            Scene?.Initialize();

            IsInitialized = true;
        }

        public void LoadContent()
        {
            ContentLibrary.LoadContent(_contentManager);

            ViewManager.Main.LoadContent(ContentLibrary);
            SongPlayer.Instance.LoadContent(ContentLibrary);

            Scene?.LoadContent(ContentLibrary);
            _sceneChanged = false;

            IsLoaded = true;
        }

        public void BeginUpdate(GameTime gameTime)
        {
            _elapsedTime.Update(gameTime);
        }

        public void HandleInput()
        {
            ControlManager.Update(_elapsedTime, FocusedClient != null);
        }

        public void Update()
        {
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
            }

            using (GlyphSchedulableBase.UpdateWatchTree.Start("Root"))
            {
                do
                {
                    if (_sceneChanged)
                    {
                        Scene?.Initialize();
                        Scene?.LoadContent(ContentLibrary);

                        _sceneChanged = false;
                    }
                    Scene?.Update(_elapsedTime);
                } while (_sceneChanged);

                ViewManager.Main.Update(_elapsedTime);
                SongPlayer.Instance.Update(_elapsedTime);
            }

            if (GlyphSchedulableBase.UpdateWatchTree.Enabled)
            {
                GlyphSchedulableBase.UpdateWatchTree.Results.First().Value.SaveToCsv("watchtree_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss"));
                GlyphSchedulableBase.UpdateWatchTree.Enabled = false;
            }
        }

        public void BeginDraw()
        {
        }

        public void Draw(DrawContext drawContext)
        {
            var drawer = new Drawer(drawContext.GraphicsDevice, drawContext.Resolution, drawContext.DefaultRenderTarget);
            drawContext.GraphicsDevice.Clear(Color.Black);

            foreach (IView view in ViewManager.Main.Views.Where(x => x.Visible && (drawContext.IsViewVisiblePredicate == null || drawContext.IsViewVisiblePredicate(x))))
            {
                drawer.CurrentView = view;
                view.PrepareDraw(drawer);
                Scene?.Draw(drawer);
            }
        }

        public void EndDraw()
        {
        }

        public void Start()
        {
            IsStarted = true;
            IsPaused = false;
            _elapsedTime.Resume();
            Logger.Info("Start engine");

            Started?.Invoke();
        }

        public void Stop()
        {
            IsStarted = false;
            IsPaused = false;
            Logger.Info("Stop engine");

            Stopped?.Invoke();
        }

        public void Pause()
        {
            IsPaused = true;
            _elapsedTime.Pause();
            Logger.Info("Pause engine");

            Paused?.Invoke();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class DrawContext
        {
            public GraphicsDevice GraphicsDevice { get; private set; }
            public Resolution Resolution { get; private set; }
            public RenderTarget2D DefaultRenderTarget { get; set; }
            public Predicate<IView> IsViewVisiblePredicate { get; set; }

            public DrawContext(GraphicsDevice graphicsDevice, Resolution resolution)
            {
                GraphicsDevice = graphicsDevice;
                Resolution = resolution;
            }
        }
    }
}