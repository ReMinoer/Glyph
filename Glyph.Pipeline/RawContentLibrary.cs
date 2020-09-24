using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using NLog;
using Simulacra.IO.Utils;
using Simulacra.IO.Watching;

namespace Glyph.Pipeline
{
    public class RawContentLibrary : ContentLibrary
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PathWatcher _pathWatcher;
        private readonly PipelineManager _pipelineManager;
        private readonly SemaphoreSlim _pipelineSemaphore = new SemaphoreSlim(1);

        public string RawRootPath { get; }

        public RawContentLibrary(IGraphicsDeviceService graphicsDeviceService, string rawRootPath, string cacheRootPath)
            : base(graphicsDeviceService, Path.Combine(cacheRootPath, "bin"))
        {
            RawRootPath = rawRootPath;
            _pathWatcher = new PathWatcher();

            // Song files are not valid if we are not using full paths
            _pipelineManager = new PipelineManager(Path.GetFullPath(RawRootPath), Path.GetFullPath(RootPath), Path.GetFullPath(Path.Combine(cacheRootPath, "obj")))
            {
                CompressContent = true,
                Profile = GraphicsProfile.HiDef,
                Platform = TargetPlatform.Windows
            };
        }

        protected override IAsset<T> CreateAsset<T>(string assetPath, LoadDelegate<T> loadDelegate)
        {
            var asset = new Asset<T>(assetPath, loadDelegate);

            // TODO: Ideally it should trigger asset refresh for all future changes on files using same asset path and any extension
            foreach (string rawFilePath in GetAllRawFilesMatchingAssetPath(assetPath))
            {
                string absoluteRawFilePath = rawFilePath;
                if (!PathUtils.IsValidAbsolutePath(absoluteRawFilePath))
                    absoluteRawFilePath = Path.Combine(Environment.CurrentDirectory, rawFilePath);

                async void OnFileChanged(object s, FileChangedEventArgs e) => await asset.ResetAsync();

                // Refresh asset on file changes
                _pathWatcher.WatchFile(absoluteRawFilePath, OnFileChanged);
                // Stop watching changes on asset full release
                asset.FullyReleasing += (s, e) => _pathWatcher.Unwatch(absoluteRawFilePath, OnFileChanged);
            }

            return asset;
        }

        protected override async Task<T> Load<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            await CookAsset(assetPath);
            return await base.Load(loadingFunc, assetPath, cancellationToken);
        }

        protected override async Task<Effect> LoadEffect(string assetPath, CancellationToken cancellationToken)
        {
            await CookAsset(assetPath);
            return await base.LoadEffect(assetPath, cancellationToken);
        }

        private async Task CookAsset(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await _pipelineSemaphore.WaitAsync();

            bool foundImporter = false;
            bool foundProcessor = false;
            try
            {
                string[] rawFilePaths = GetAllRawFilesMatchingAssetPath(assetPath);
                if (rawFilePaths.Length == 0)
                    throw new AssetNotFoundException(assetPath);

                foreach (string rawFilePath in rawFilePaths)
                {
                    string importerName = _pipelineManager.FindImporterByExtension(Path.GetExtension(rawFilePath));
                    if (importerName == null)
                        continue;

                    foundImporter = true;

                    string processorName = _pipelineManager.FindDefaultProcessor(importerName);
                    if (processorName == null)
                        continue;

                    foundProcessor = true;

                    string inputPath = Path.GetFullPath(rawFilePath);
                    string outputPath = Path.GetFullPath(Path.Combine(RootPath, rawFilePath.Substring(RawRootPath.Length + 1)));

                    await Task.Run(() => _pipelineManager.BuildContent(inputPath, outputPath, importerName, processorName));

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
                throw new NoImporterException(assetPath);
            if (!foundProcessor)
                throw new NoProcessorException(assetPath);
        }

        private string[] GetAllRawFilesMatchingAssetPath(string assetPath) => Directory.GetFiles(RawRootPath, $"{assetPath}.*");
    }
}