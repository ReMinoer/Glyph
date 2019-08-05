using System;
using System.IO;
using System.Linq;
using Niddle;
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Application;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Graphics;
using Glyph.Messaging;
using Glyph.Resolver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Engine
{
    public class GlyphEngine
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ContentManager _contentManager;
        private readonly ElapsedTime _elapsedTime = new ElapsedTime();
        private IGlyphClient _focusedClient;
        public bool IsInitialized { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public IDependencyRegistry Registry { get; }
        public IDependencyResolver Resolver { get; }
        public RootView RootView { get; private set; }
        public ProjectionManager ProjectionManager { get; }
        public ContentLibrary ContentLibrary { get; }
        public InputClientManager InputClientManager { get; }
        public InteractionManager InteractionManager { get; }
        private GlyphObject _root;

        public GlyphObject Root
        {
            get
            {
                if (_root == null)
                    Root = Resolver.Resolve<GlyphObject>();

                return _root;
            }
            set
            {
                _root = value;
                _root.Router.Global = Resolver.Resolve<TrackingRouter>();
            }
        }

        public IGlyphClient FocusedClient
        {
            get => _focusedClient;
            set
            {
                if (_focusedClient == value)
                    return;

                _focusedClient = value;

                RootView.DrawClient = _focusedClient;
                InputClientManager.DrawClient = _focusedClient;
                InputClientManager.InputClient = _focusedClient;

                FocusChanged?.Invoke(_focusedClient);
            }
        }
        
        public event Action Started;
        public event Action Stopped;
        public event Action Paused;
        public event Action<IGlyphClient> FocusChanged;

        public GlyphEngine(ContentManager contentManager, Action<IDependencyRegistry> dependencyConfigurator = null, params string[] args)
        {
            Logger.Info("Engine arguments : " + string.Join(" ", args));

            _contentManager = contentManager;

            Registry = GlyphRegistry.BuildGlobalRegistry();
            dependencyConfigurator?.Invoke(Registry);

            var resolver = new RegistryResolver(Registry);
            Registry.Add(GlyphDependency.OnType<RegistryResolver>().Using(resolver));
            Resolver = resolver;

            RootView = new RootView();
            ProjectionManager = new ProjectionManager(RootView, Resolver.Resolve<ISubscribableRouter>());

            _contentManager.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();
            InputClientManager = new InputClientManager();
            InteractionManager = new InteractionManager();

            Registry.Add(GlyphDependency.OnType<GlyphEngine>().Using(this));
            Registry.Add(GlyphDependency.OnType<RootView>().Using(RootView));
            Registry.Add(GlyphDependency.OnType<ProjectionManager>().Using(ProjectionManager));
            Registry.Add(GlyphDependency.OnType<ContentLibrary>().Using(ContentLibrary));
            Registry.Add(GlyphDependency.OnType<InputClientManager>().Using(InputClientManager));
            Registry.Add(GlyphDependency.OnType<InteractionManager>().Using(InteractionManager));
            Registry.Add(GlyphDependency.OnType<Func<GraphicsDevice>>().Using(() =>
            {
                if (FocusedClient != null)
                {
                    return FocusedClient.GraphicsDevice;
                }
                
                return ((IGraphicsDeviceService)contentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            }));
        }

        public void Initialize()
        {
            RootView?.Initialize();
            Root?.Initialize();

            IsInitialized = true;
        }

        public void LoadContent()
        {
            ContentLibrary.LoadContent(_contentManager);
            SongPlayer.Instance.LoadContent(ContentLibrary);

            Root?.LoadContent(ContentLibrary);

            IsLoaded = true;
        }

        public void BeginUpdate(GameTime gameTime)
        {
            _elapsedTime.Update(gameTime);
        }

        public void HandleInput()
        {
            if (InputClientManager.InputClient != null)
                InteractionManager.Update(_elapsedTime.UnscaledDelta);
            else
                InputManager.Instance.InputStates?.Ignore();
        }

        public void Update()
        {
            using (GlyphObject.UpdateWatchTree.Start("Root"))
            {
                Root?.Update(_elapsedTime);
                SongPlayer.Instance.Update(_elapsedTime);
            }

            if (GlyphObject.UpdateWatchTree.Enabled)
            {
                using (var streamWriter = new StreamWriter("watchtree_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss")))
                    GlyphObject.UpdateWatchTree.Results.First().Value.WriteAsCsv(streamWriter);
                GlyphObject.UpdateWatchTree.Enabled = false;
            }
        }

        public void BeginDraw()
        {
        }

        public void Draw(IDrawClient drawClient)
        {
            var spriteBatch = new SpriteBatch(drawClient.GraphicsDevice);
            var drawer = new Drawer(new SpriteBatchStack(spriteBatch), drawClient, Root, RootView.Camera.GetSceneNode().RootNode())
            {
                CurrentView = RootView
            };

            RootView.Draw(drawer);
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
            _elapsedTime.Pause();
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
    }
}