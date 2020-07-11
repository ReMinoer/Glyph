using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using Simulacra.IO.Utils;

namespace Glyph
{
    public class ContentLibrary : IContentLibrary
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentBag<ContentManager> _contentManagers = new ConcurrentBag<ContentManager>();
        private readonly ConcurrentDictionary<string, Task<object>> _loadingTasks = new ConcurrentDictionary<string, Task<object>>(new PathComparer());

        private readonly IGraphicsDeviceService _graphicsDeviceService;
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _effectLock = new SemaphoreSlim(1);

        private string _rootPath;
        public string RootPath
        {
            get => _rootPath;
            set
            {
                if (string.Equals(_rootPath, value, StringComparison.OrdinalIgnoreCase))
                    return;

                _rootPath = value;
            }
        }

        public ContentLibrary(IGraphicsDeviceService graphicsDeviceService, string rootPath = null)
        {
            _graphicsDeviceService = graphicsDeviceService;
            _serviceProvider = new ServiceProvider(graphicsDeviceService);

            RootPath = rootPath ?? "Content";
        }

        public async Task<T> GetOrLoad<T>(string assetPath)
        {
            return (T)await _loadingTasks.GetOrAdd(assetPath, x => Task.Run(() => Load<T>(x)));
        }

        public async Task<T> GetOrLoadLocalized<T>(string assetPath)
        {
            return (T)await _loadingTasks.GetOrAdd(assetPath, x => Task.Run(() => LoadLocalized<T>(x)));
        }

        public async Task<Effect> GetOrLoadEffect(string assetPath)
        {
            return (Effect)await _loadingTasks.GetOrAdd(assetPath, LoadEffect);
        }

        private object Load<T>(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ContentManager contentManager = GetContentManager();

            var content = contentManager.Load<T>(assetPath);

            ReleaseContentManager(contentManager);
            LogLoadingTime(assetPath, stopwatch);

            return content;
        }

        private object LoadLocalized<T>(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ContentManager contentManager = GetContentManager();

            var content = contentManager.LoadLocalized<T>(assetPath);

            ReleaseContentManager(contentManager);
            LogLoadingTime(assetPath, stopwatch);

            return content;
        }

        private async Task<object> LoadEffect(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Effect effect;
            await _effectLock.WaitAsync();
            try
            {
                effect = new Effect(_graphicsDeviceService.GraphicsDevice, File.ReadAllBytes(Path.Combine(_rootPath, assetPath + ".mgfx")));
            }
            finally
            {
                _effectLock.Release();
            }

            LogLoadingTime(assetPath, stopwatch);

            return effect;
        }

        private void LogLoadingTime(string assetRelativePath, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            Logger.Info($"Loaded {assetRelativePath} ({stopwatch.ElapsedMilliseconds} ms)");
        }

        private ContentManager GetContentManager()
        {
            if (!_contentManagers.TryTake(out ContentManager contentManager))
                contentManager = new ContentManager(_serviceProvider, _rootPath);
            return contentManager;
        }

        private void ReleaseContentManager(ContentManager contentManager)
        {
            _contentManagers.Add(contentManager);
        }

        public class ServiceProvider : IServiceProvider
        {
            public IGraphicsDeviceService GraphicsDeviceService { get; }

            public ServiceProvider(IGraphicsDeviceService graphicsDeviceService)
            {
                GraphicsDeviceService = graphicsDeviceService;
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IGraphicsDeviceService))
                    return GraphicsDeviceService;

                throw new InvalidOperationException();
            }
        }
    }
}