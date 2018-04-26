using System;
using System.IO;
using System.Linq;
using Diese.Collections;
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
        public IDependencyInjector Injector { get; }
        public ViewManager ViewManager { get; }
        public ContentLibrary ContentLibrary { get; }
        public InputClientManager InputClientManager { get; }
        public ControlManager ControlManager { get; }
        private GlyphObject _root;
        private IControl _mute;
        private IControl _xboxQuit;

        public GlyphObject Root
        {
            get => _root ?? (_root = Injector.Resolve<GlyphObject>());
            set => _root = value;
        }

        public IGlyphClient FocusedClient
        {
            get { return _focusedClient; }
            set
            {
                if (_focusedClient == value)
                    return;

                _focusedClient = value;
                InputClientManager.Current = _focusedClient;
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

            Registry = new GlyphRegistry();
            dependencyConfigurator?.Invoke(Registry);

            var injector = new RegistryInjector(Registry);
            Registry.RegisterInstance<RegistryInjector>(injector);
            Injector = injector;

            ViewManager = new ViewManager();

            _contentManager.RootDirectory = "Content";
            ContentLibrary = new ContentLibrary();
            InputClientManager = new InputClientManager();
            ControlManager = new ControlManager();
            ControlManager.ApplyProfile(ControlLayerSchedulerProfile.Get().GraphData);

            ControlManager.Plan(new ControlLayer("Program controls", ControlLayerTag.Debug)).RegisterMany(new []
            {
                _mute = new Control("Mute", InputSystem.Instance.Keyboard[Keys.F10]),
                _xboxQuit = new ControlSimultaneous<IControl>("Quit game")
                {
                    Components =
                    {
                        new Control(InputSystem.Instance[PlayerIndex.One][GamePadButton.Start]),
                        new Control(InputSystem.Instance[PlayerIndex.One][GamePadButton.Back])
                    }
                }
            });

            Registry.RegisterInstance<GlyphEngine>(this);
            Registry.RegisterInstance<ViewManager>(ViewManager);
            Registry.RegisterInstance<ContentLibrary>(ContentLibrary);
            Registry.RegisterInstance<InputClientManager>(InputClientManager);
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
            ViewManager.Initialize();
            Root?.Initialize();

            IsInitialized = true;
        }

        public void LoadContent()
        {
            ContentLibrary.LoadContent(_contentManager);

            ViewManager.LoadContent(ContentLibrary);
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
            if (InputClientManager.Current != null)
                ControlManager.Update(_elapsedTime.UnscaledDelta);
            else
                InputManager.Instance.InputStates?.Ignore();
        }

        public void Update()
        {
            if (_mute.IsActive())
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

            if (_xboxQuit.IsActive())
                Stop();

            using (GlyphObject.UpdateWatchTree.Start("Root"))
            {
                Root?.Update(_elapsedTime);
                ViewManager.Update(_elapsedTime);
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
            var drawer = new Drawer(drawClient);
            drawer.GraphicsDevice.Clear(Color.Black);

            foreach (IView view in ViewManager.Views.Where(x => x.Displayed(drawClient)))
            {
                drawer.CurrentView = view;
                view.PrepareDraw(drawer);
                Root?.Draw(drawer);
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