using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Simulacra.IO.Utils;

namespace Glyph.Content
{
    public class ContentLibrary : IContentLibrary
    {
        private readonly ConcurrentDictionary<string, IAsset> _assetCache = new ConcurrentDictionary<string, IAsset>(new PathComparer());
        private readonly ConcurrentDictionary<string, ContentManager> _usedContentManagers = new ConcurrentDictionary<string, ContentManager>(new PathComparer());
        private readonly ContentManagerPool _contentManagerPool;

        private readonly IGraphicsDeviceService _graphicsDeviceService;
        protected readonly ILogger Logger;

        private readonly SemaphoreSlim _effectLock = new SemaphoreSlim(1);

        public string RootPath { get; }
        protected virtual string WorkingDirectory => RootPath;
        string IContentLibrary.WorkingDirectory => WorkingDirectory;

        public ContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger, string rootPath = null)
        {
            _graphicsDeviceService = graphicsDeviceService;
            Logger = logger;

            RootPath = rootPath ?? "Content";

            var serviceProvider = new ServiceProvider(graphicsDeviceService);
            _contentManagerPool = new ContentManagerPool(serviceProvider, RootPath);
        }

        public IAsset<T> GetAsset<T>(string assetPath)
        {
            return (IAsset<T>)_assetCache.GetOrAdd(assetPath, x =>
            {
                IAsset<T> asset = CreateAsset<T>(x);
                asset.ContentChanged += OnAssetContentChanged;
                asset.FullyReleasing += OnAssetFullyReleasing;
                return asset;
            });
        }

        public IAsset<T> GetLocalizedAsset<T>(string assetPath)
        {
            return (IAsset<T>)_assetCache.GetOrAdd(assetPath, x =>
            {
                IAsset<T> asset = CreateLocalizedAsset<T>(x);
                asset.FullyReleasing += OnAssetFullyReleasing;
                return asset;
            });
        }

        public IAsset<Effect> GetEffectAsset(string assetPath)
        {
            return (IAsset<Effect>)_assetCache.GetOrAdd(assetPath, x =>
            {
                IAsset<Effect> asset = CreateEffectAsset(x);
                asset.FullyReleasing += OnAssetFullyReleasing;
                return asset;
            });
        }

        private IAsset<T> CreateAsset<T>(string assetPath)
        {
            return CreateAsset(assetPath,
                (path, token) => LoadAsync((c, a) => c.Load<T>(a), path, token),
                (path, token) => Load((c, a) => c.Load<T>(a), path, token));
        }

        private IAsset<T> CreateLocalizedAsset<T>(string assetPath)
        {
            return CreateAsset(assetPath,
                (path, token) => LoadAsync((c, a) => c.LoadLocalized<T>(a), path, token),
                (path, token) => Load((c, a) => c.LoadLocalized<T>(a), path, token));
        }

        private IAsset<Effect> CreateEffectAsset(string assetPath)
        {
            return CreateAsset(assetPath, LoadEffectAsync, LoadEffect);
        }

        protected virtual IAsset<T> CreateAsset<T>(string assetPath, LoadAsyncDelegate<T> loadAsyncDelegate, LoadDelegate<T> loadDelegate)
        {
            return new Asset<T>(assetPath, loadAsyncDelegate, loadDelegate);
        }

        protected virtual T Load<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            return LoadImplementation(loadingFunc, assetPath, cancellationToken);
        }

        protected virtual Task<T> LoadAsync<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            return Task.FromResult(LoadImplementation(loadingFunc, assetPath, cancellationToken));
        }

        private T LoadImplementation<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                stopwatch = Stopwatch.StartNew();
                ContentManager contentManager = _contentManagerPool.GetContentManager();
                try
                {
                    T content = loadingFunc(contentManager, assetPath);

                    cancellationToken.ThrowIfCancellationRequested();

                    _usedContentManagers.TryAdd(assetPath, contentManager);
                    LogLoadingTime(assetPath, stopwatch);

                    return content;
                }
                catch (Exception)
                {
                    TryReconditionContentManager(assetPath);
                    throw;
                }
            }
            catch (OperationCanceledException)
            {
                LogCancellationTime(assetPath, stopwatch);
                throw;
            }
        }

        protected virtual Effect LoadEffect(string assetPath, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = null;
            try
            {
                _effectLock.Wait(cancellationToken);

                stopwatch = Stopwatch.StartNew();

                string effectFilePath = Path.Combine(RootPath, assetPath + ".mgfx");

                using (FileStream fileStream = File.OpenRead(effectFilePath))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);

                    cancellationToken.ThrowIfCancellationRequested();
                    LogLoadingTime(assetPath, stopwatch);

                    return new Effect(_graphicsDeviceService.GraphicsDevice, bytes);
                }
            }
            catch (OperationCanceledException)
            {
                LogCancellationTime(assetPath, stopwatch);
                throw;
            }
            finally
            {
                _effectLock.Release();
            }
        }

        protected virtual async Task<Effect> LoadEffectAsync(string assetPath, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = null;
            try
            {
                await _effectLock.WaitAsync(cancellationToken);

                stopwatch = Stopwatch.StartNew();

                string effectFilePath = Path.Combine(RootPath, assetPath + ".mgfx");

                using (FileStream fileStream = File.OpenRead(effectFilePath))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(bytes, 0, (int)fileStream.Length, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();
                    LogLoadingTime(assetPath, stopwatch);

                    return new Effect(_graphicsDeviceService.GraphicsDevice, bytes);
                }
            }
            catch (OperationCanceledException)
            {
                LogCancellationTime(assetPath, stopwatch);
                throw;
            }
            finally
            {
                _effectLock.Release();
            }
        }

        private void LogLoadingTime(string assetRelativePath, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            Logger.Info($"Loaded {assetRelativePath} ({stopwatch.ElapsedMilliseconds} ms)");
        }

        private void LogCancellationTime(string assetRelativePath, Stopwatch stopwatch)
        {
            stopwatch?.Stop();
            Logger.Info($"Canceled loading of {assetRelativePath} ({stopwatch?.ElapsedMilliseconds ?? 0} ms)");
        }

        private void OnAssetContentChanged(object sender, EventArgs e)
        {
            var asset = (IAsset)sender;
            TryReconditionContentManager(asset.AssetPath);
        }

        private void OnAssetFullyReleasing(object sender, EventArgs e)
        {
            var asset = (IAsset)sender;
            asset.FullyReleasing -= OnAssetFullyReleasing;
            asset.ContentChanged -= OnAssetContentChanged;

            _assetCache.TryRemove(asset.AssetPath, out _);
            TryReconditionContentManager(asset.AssetPath);

            Logger.Info($"Unloaded {asset.AssetPath}");
        }

        private void TryReconditionContentManager(string assetPath)
        {
            if (_usedContentManagers.TryRemove(assetPath, out ContentManager contentManager))
                _contentManagerPool.ReleaseContentManager(contentManager);
        }

        private class ContentManagerPool
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly string _rootPath;

            private readonly ConcurrentBag<ContentManager> _pool = new ConcurrentBag<ContentManager>();

            public ContentManagerPool(IServiceProvider serviceProvider, string rootPath)
            {
                _serviceProvider = serviceProvider;
                _rootPath = rootPath;
            }

            public ContentManager GetContentManager()
            {
                if (!_pool.TryTake(out ContentManager contentManager))
                    contentManager = new ContentManager(_serviceProvider, _rootPath);

                return contentManager;
            }

            public void ReleaseContentManager(ContentManager contentManager)
            {
                Task.Run(() =>
                {
                    contentManager.Unload();
                    _pool.Add(contentManager);
                });
            }
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