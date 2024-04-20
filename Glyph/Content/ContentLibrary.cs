using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        protected readonly ILogger Logger;

        public string RootPath { get; }
        protected virtual string WorkingDirectory => RootPath;
        string IContentLibrary.WorkingDirectory => WorkingDirectory;

        public ContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger, string rootPath = null)
        {
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

        private IAsset<T> CreateAsset<T>(string assetPath)
        {
            TryReconditionContentManager(assetPath);

            return CreateAsset(assetPath,
                (path, token) => LoadAsync((c, a) => c.Load<T>(a), path, token),
                (path, token) => Load((c, a) => c.Load<T>(a), path, token));
        }

        private IAsset<T> CreateLocalizedAsset<T>(string assetPath)
        {
            TryReconditionContentManager(assetPath);

            return CreateAsset(assetPath,
                (path, token) => LoadAsync((c, a) => c.LoadLocalized<T>(a), path, token),
                (path, token) => Load((c, a) => c.LoadLocalized<T>(a), path, token));
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

        private void OnAssetFullyReleasing(object sender, EventArgs e)
        {
            var asset = (IAsset)sender;
            asset.FullyReleasing -= OnAssetFullyReleasing;

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