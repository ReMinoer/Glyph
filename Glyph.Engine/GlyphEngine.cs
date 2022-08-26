using System;
using System.Linq;
using System.Threading.Tasks;
using Niddle;
using Fingear;
using Fingear.Inputs;
using Glyph.Application;
using Glyph.Content;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Graphics;
using Glyph.Messaging;
using Glyph.Resolver;
using Glyph.Scheduling;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Engine
{
    public class GlyphEngine
    {
        private readonly ElapsedTime _elapsedTime = new ElapsedTime();
        private IGlyphClient _focusedClient;
        public bool IsInitialized { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public IDependencyRegistry Registry { get; }
        public IDependencyResolver Resolver { get; }
        public ILogger Logger { get; }
        public RootView RootView { get; private set; }
        public ProjectionManager ProjectionManager { get; }
        public InputClientManager InputClientManager { get; }
        public InteractionManager InteractionManager { get; }
        public CursorManager CursorManager { get; }

        public UpdateScheduler UpdateScheduler { get; }
        public DrawScheduler DrawScheduler { get; }

        private SpriteBatch _spriteBatch;

        private readonly IGraphicsDeviceService _graphicsDeviceService;

        private IContentLibrary _contentLibrary;
        public IContentLibrary ContentLibrary
        {
            get => _contentLibrary ?? (_contentLibrary = new ContentLibrary(_graphicsDeviceService, Logger));
        }

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
                InteractionManager.Reset();

                FocusChanged?.Invoke(_focusedClient);
            }
        }
        
        public event Action Started;
        public event Action Stopped;
        public event Action Paused;
        public event Action<IGlyphClient> FocusChanged;

        public GlyphEngine(IGraphicsDeviceService graphicsDeviceService, IContentLibrary contentLibrary, ILogger logger,
            Action<IDependencyRegistry> dependencyConfigurator = null, params string[] args)
        {
            logger.LogInformation("Engine arguments : " + string.Join(" ", args));

            _graphicsDeviceService = graphicsDeviceService;
            _contentLibrary = contentLibrary;
            Logger = logger;

            Registry = GlyphRegistry.BuildGlobalRegistry();
            dependencyConfigurator?.Invoke(Registry);

            var resolver = new RegistryResolver(Registry);
            Registry.Add(GlyphDependency.OnType<RegistryResolver>().Using(resolver));
            Resolver = resolver;

            RootView = new RootView();
            ProjectionManager = new ProjectionManager(RootView, Resolver.Resolve<ISubscribableRouter>());

            InputClientManager = new InputClientManager();
            InteractionManager = new InteractionManager();
            CursorManager = new CursorManager();

            UpdateScheduler = resolver.Resolve<UpdateScheduler>();
            DrawScheduler = resolver.Resolve<DrawScheduler>();

            Registry.Add(GlyphDependency.OnType<GlyphEngine>().Using(this));
            Registry.Add(GlyphDependency.OnType<ILogger>().Using(Logger));
            Registry.Add(GlyphDependency.OnType<RootView>().Using(RootView));
            Registry.Add(GlyphDependency.OnType<ProjectionManager>().Using(ProjectionManager));
            Registry.Add(GlyphDependency.OnType<IContentLibrary>().Using(ContentLibrary));
            Registry.Add(GlyphDependency.OnType<InputClientManager>().Using(InputClientManager));
            Registry.Add(GlyphDependency.OnType<InteractionManager>().Using(InteractionManager));
            Registry.Add(GlyphDependency.OnType<CursorManager>().Using(CursorManager));
            Registry.Add(GlyphDependency.OnType<Func<GraphicsDevice>>().Using(() =>
            {
                if (FocusedClient != null)
                {
                    return FocusedClient.GraphicsDevice;
                }
                
                return _graphicsDeviceService.GraphicsDevice;
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
            if (_spriteBatch is null)
            {
                GraphicsDevice graphicsDevice = Resolver.Resolve<Func<GraphicsDevice>>()();
                _spriteBatch = new SpriteBatch(graphicsDevice);
            }

            if (Root != null)
                Root.LoadContent(ContentLibrary);

            IsLoaded = true;
        }

        public async Task LoadContentAsync()
        {
            if (_spriteBatch is null)
            {
                GraphicsDevice graphicsDevice = Resolver.Resolve<Func<GraphicsDevice>>()();
                _spriteBatch = new SpriteBatch(graphicsDevice);
            }

            if (Root != null)
                await Root.LoadContentAsync(ContentLibrary);

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
            foreach (IUpdateTask updateTask in UpdateScheduler.Schedule.Where(x => x.Active))
                updateTask.Update(_elapsedTime);
        }

        public void BeginDraw()
        {
        }

        public void Draw(IDrawClient drawClient)
        {
            var drawer = new Drawer(DrawScheduler, new SpriteBatchStack(_spriteBatch), drawClient, RootView.Camera.GetSceneNode().RootNode())
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
            if (IsStarted && !IsPaused)
                return;

            IsStarted = true;
            IsPaused = false;
            _elapsedTime.Resume();
            Logger.Info("Start engine");

            Started?.Invoke();
        }

        public void Stop()
        {
            if (!IsStarted)
                return;

            IsStarted = false;
            IsPaused = false;
            _elapsedTime.Pause();
            Logger.Info("Stop engine");

            Stopped?.Invoke();
        }

        public void Pause()
        {
            if (IsPaused)
                return;

            IsPaused = true;
            _elapsedTime.Pause();
            Logger.Info("Pause engine");

            Paused?.Invoke();
        }

        public void PauseOnNextFrame()
        {
            Pause();
            _elapsedTime.RequestNextFrame();
        }
    }
}