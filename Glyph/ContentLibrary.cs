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
        private readonly ConcurrentDictionary<string, Task> _loadingTasks = new ConcurrentDictionary<string, Task>(new PathComparer());
        
        private readonly SemaphoreSlim _effectLock = new SemaphoreSlim(1);

        public IServiceProvider ServiceProvider { get; }

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

        public ContentLibrary(IServiceProvider serviceProvider, string rootPath = null)
        {
            ServiceProvider = serviceProvider;
            RootPath = rootPath;
        }

        public Task<T> GetOrLoad<T>(string assetPath)
        {
            return (Task<T>)_loadingTasks.GetOrAdd(assetPath, x => Task.Run(() => Load<T>(x)));
        }

        public Task<T> GetOrLoadLocalized<T>(string assetPath)
        {
            return (Task<T>)_loadingTasks.GetOrAdd(assetPath, x => Task.Run(() => LoadLocalized<T>(x)));
        }

        public Task<Effect> GetOrLoadEffect(string assetPath)
        {
            return (Task<Effect>)_loadingTasks.GetOrAdd(assetPath, LoadEffect);
        }

        private T Load<T>(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ContentManager contentManager = GetContentManager();

            var content = contentManager.Load<T>(assetPath);

            ReleaseContentManager(contentManager);
            LogLoadingTime(assetPath, stopwatch);

            return content;
        }

        private T LoadLocalized<T>(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ContentManager contentManager = GetContentManager();

            var content = contentManager.LoadLocalized<T>(assetPath);

            ReleaseContentManager(contentManager);
            LogLoadingTime(assetPath, stopwatch);

            return content;
        }

        private async Task<Effect> LoadEffect(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            Effect effect;
            await _effectLock.WaitAsync();
            try
            {
                effect = new Effect(graphicsDevice, File.ReadAllBytes(Path.Combine(_rootPath, assetPath + ".mgfx")));
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
                contentManager = new ContentManager(ServiceProvider, _rootPath);
            return contentManager;
        }

        private void ReleaseContentManager(ContentManager contentManager)
        {
            _contentManagers.Add(contentManager);
        }
    }
}