using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Diese.Collections;
using Diese.Injection;
using Glyph.Application;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Annotations;
using Glyph.Core;
using Glyph.Core.ControlLayers;
using Glyph.Graphics;
using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace Glyph.Engine
{
    public class GlyphEngine : INotifyPropertyChanged
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
        public ContentLibrary ContentLibrary { get; }
        public ControlManager ControlManager { get; }
        private GlyphObject _root;

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
                ControlManager.InputClient = value;
                FocusChanged?.Invoke(_focusedClient);
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
            Root?.Initialize();

            IsInitialized = true;
        }

        public void LoadContent()
        {
            ContentLibrary.LoadContent(_contentManager);

            ViewManager.Main.LoadContent(ContentLibrary);
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
            ControlManager.Update(_elapsedTime, FocusedClient != null);
        }

        public void Update()
        {
            DeveloperControls developerControls;
            if (ControlManager.Layers.Any(out developerControls))
            {
                if (developerControls.Mute.IsActive())
                    MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

                if (developerControls.XboxQuit.IsActive())
                    Stop();

                if (developerControls.UpdateSnapshot.IsActive())
                    GlyphObject.UpdateWatchTree.Enabled = true;

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

            using (GlyphObject.UpdateWatchTree.Start("Root"))
            {
                Root?.Update(_elapsedTime);
                ViewManager.Main.Update(_elapsedTime);
                SongPlayer.Instance.Update(_elapsedTime);
            }

            if (GlyphObject.UpdateWatchTree.Enabled)
            {
                GlyphObject.UpdateWatchTree.Results.First().Value.SaveToCsv("watchtree_" + DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss"));
                GlyphObject.UpdateWatchTree.Enabled = false;
            }
        }

        public void BeginDraw()
        {
        }

        public void Draw(IGlyphClient glyphClient)
        {
            var drawer = new Drawer(glyphClient);
            drawer.GraphicsDevice.Clear(Color.Black);

            foreach (IView view in ViewManager.Main.Views.Where(x => x.Displayed(glyphClient)))
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

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}