using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using NLog;
using Simulacra.IO.Utils;

namespace Glyph.Pipeline
{
    public class RawContentLibrary : IContentLibrary
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IContentLibrary _cacheLibrary;
        private readonly ConcurrentDictionary<string, Task<object>> _loadingTasks = new ConcurrentDictionary<string, Task<object>>(new PathComparer());

        private readonly PipelineManager _pipelineManager;
        private readonly SemaphoreSlim _pipelineSemaphore = new SemaphoreSlim(1);

        private string _rootPath;
        public string RootPath
        {
            get => _rootPath;
            set
            {
                if (PathComparer.Equals(_rootPath, value, PathCaseComparison.EnvironmentDefault, FolderPathEquality.RespectAmbiguity))
                    return;

                _rootPath = value;
            }
        }

        public RawContentLibrary(IGraphicsDeviceService graphicsDeviceService, string rootPath, string cacheRootPath)
        {
            RootPath = rootPath;
            string cacheBinPath = Path.Combine(cacheRootPath, "bin");
            string cacheObjPath = Path.Combine(cacheRootPath, "obj");

            _cacheLibrary = new ContentLibrary(graphicsDeviceService, cacheBinPath);
            _pipelineManager = new PipelineManager(rootPath, cacheBinPath, cacheObjPath)
            {
                CompressContent = true,
                Profile = GraphicsProfile.HiDef,
                Platform = TargetPlatform.Windows
            };
        }

        public async Task<T> GetOrLoad<T>(string assetPath)
        {
            return (T)await _loadingTasks.GetOrAdd(assetPath, Load<T>);
        }

        public async Task<T> GetOrLoadLocalized<T>(string assetPath)
        {
            return (T)await _loadingTasks.GetOrAdd(assetPath, LoadLocalized<T>);
        }

        public async Task<Effect> GetOrLoadEffect(string assetPath)
        {
            return (Effect)await _loadingTasks.GetOrAdd(assetPath, LoadEffect);
        }

        private async Task<object> Load<T>(string assetPath)
        {
            await CookAsset(assetPath);
            return await _cacheLibrary.GetOrLoad<T>(assetPath);
        }

        private async Task<object> LoadLocalized<T>(string assetPath)
        {
            await CookAsset(assetPath);
            return await _cacheLibrary.GetOrLoadLocalized<T>(assetPath);
        }

        private async Task<object> LoadEffect(string assetPath)
        {
            await CookAsset(assetPath);
            return await _cacheLibrary.GetOrLoadEffect(assetPath);
        }

        private async Task CookAsset(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await _pipelineSemaphore.WaitAsync();

            bool foundImporter = false;
            bool foundProcessor = false;
            try
            {
                foreach (string sourcePath in Directory.EnumerateFiles(RootPath, $"{assetPath}.*"))
                {
                    string importerName = _pipelineManager.FindImporterByExtension(Path.GetExtension(sourcePath));
                    if (importerName == null)
                        continue;

                    foundImporter = true;

                    string processorName = _pipelineManager.FindDefaultProcessor(importerName);
                    if (processorName == null)
                        continue;

                    foundProcessor = true;

                    string outputPath = sourcePath.Substring(RootPath.Length + 1);
                    await Task.Run(() => _pipelineManager.BuildContent(sourcePath, outputPath, importerName, processorName));

                    Logger.Info($"Cooked {assetPath} ({stopwatch.ElapsedMilliseconds} ms, Importer: {importerName}, Processor: {processorName})");
                    return;
                }
            }
            finally
            {
                stopwatch.Stop();
                _pipelineSemaphore.Release();
            }

            if (!foundImporter)
                throw new PipelineException($"No importer found for {assetPath}");
            if (!foundProcessor)
                throw new PipelineException($"No processor found for {assetPath}");
        }
    }
}