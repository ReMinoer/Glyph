using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace Glyph
{
    public class ContentLibrary : IContentLibrary
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentBag<ContentManager> _contentManagers = new ConcurrentBag<ContentManager>();
        private readonly ConcurrentDictionary<string, Task> _loadingTasks = new ConcurrentDictionary<string, Task>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _assetPathByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _effectPathByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        private SemaphoreSlim _effectLock = new SemaphoreSlim(1);

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
                RegeneratePathsDictionaries();
            }
        }

        public ContentLibrary(IServiceProvider serviceProvider, string rootPath = null)
        {
            ServiceProvider = serviceProvider;
            RootPath = rootPath;
        }

        public Task<T> GetOrLoad<T>(string assetName)
        {
            return (Task<T>)_loadingTasks.GetOrAdd(_assetPathByName[assetName], x => Task.Run(() => Load<T>(x)));
        }

        public Task<T> GetOrLoadLocalized<T>(string assetName)
        {
            return (Task<T>)_loadingTasks.GetOrAdd(_assetPathByName[assetName], x => Task.Run(() => LoadLocalized<T>(x)));
        }

        public Task<Effect> GetOrLoadEffect(string assetName)
        {
            return (Task<Effect>)_loadingTasks.GetOrAdd(_effectPathByName[assetName], LoadEffect);
        }

        private T Load<T>(string assetRelativePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ContentManager contentManager = GetContentManager();

            var content = contentManager.Load<T>(assetRelativePath);

            ReleaseContentManager(contentManager);
            LogLoadingTime(assetRelativePath, stopwatch);

            return content;
        }

        private T LoadLocalized<T>(string assetRelativePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ContentManager contentManager = GetContentManager();

            var content = contentManager.LoadLocalized<T>(assetRelativePath);

            ReleaseContentManager(contentManager);
            LogLoadingTime(assetRelativePath, stopwatch);

            return content;
        }

        private async Task<Effect> LoadEffect(string assetRelativePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            Effect effect;
            await _effectLock.WaitAsync();
            try
            {
                effect = new Effect(graphicsDevice, File.ReadAllBytes(Path.Combine(_rootPath, assetRelativePath + ".mgfx")));
            }
            finally
            {
                _effectLock.Release();
            }

            LogLoadingTime(assetRelativePath, stopwatch);

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

        private void RegeneratePathsDictionaries()
        {
            RegeneratePathDictionary(_assetPathByName, "xnb");
            RegeneratePathDictionary(_effectPathByName, "mgfx");
        }

        private void RegeneratePathDictionary(Dictionary<string, string> dictionary, string extension)
        {
            dictionary.Clear();
            if (RootPath == null)
                return;

            IEnumerable<string> filePaths = Directory.GetFiles(RootPath, $"*.{extension}", SearchOption.AllDirectories)
                                                     .Select(x => x.Substring(RootPath.Length + 1, x.Length - $".{extension}".Length - RootPath.Length - 1).Replace('\\', '/'));

            foreach (string filePath in filePaths)
                dictionary.Add(Path.GetFileName(filePath), filePath);
        }
    }
}