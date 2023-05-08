using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Content;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using Simulacra.IO;
using Simulacra.IO.Watching;

namespace Glyph.Pipeline
{
    public class RawContentLibrary : ContentLibrary, IRawContentLibrary
    {
        private readonly PathWatcher _pathWatcher;

        private readonly PipelineManager _pipelineManager;
        private readonly SemaphoreSlim _pipelineSemaphore = new SemaphoreSlim(1);

        public string IntermediateRootPath { get; }
        public string RawRootPath { get; }
        protected override string WorkingDirectory => RawRootPath;

        public RawContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger, TargetPlatform targetPlatform, string rawRootPath, string cacheRootPath)
            : base(graphicsDeviceService, logger, Path.Combine(cacheRootPath, targetPlatform.ToString(), "bin"))
        {
            Directory.CreateDirectory(cacheRootPath);

            IntermediateRootPath = Path.Combine(cacheRootPath, targetPlatform.ToString(), "obj");
            RawRootPath = rawRootPath;

            _pathWatcher = new PathWatcher();

            // Song files are not valid if we are not using full paths
            _pipelineManager = new PipelineManager(Path.GetFullPath(RawRootPath), Path.GetFullPath(RootPath), Path.GetFullPath(IntermediateRootPath))
            {
                CompressContent = true,
                Profile = GraphicsProfile.HiDef,
                Platform = targetPlatform
            };
        }

        protected override IAsset<T> CreateAsset<T>(string assetPath, LoadAsyncDelegate<T> loadAsyncDelegate, LoadDelegate<T> loadDelegate)
        {
            var asset = new Asset<T>(assetPath, loadAsyncDelegate, loadDelegate);

            string watchedPath = Path.Combine(RawRootPath, assetPath + ".*");
            if (!PathSystem.Instance.IsValidAbsolutePath(watchedPath))
                watchedPath = Path.Combine(Environment.CurrentDirectory, watchedPath);

            _pathWatcher.WatchFile(watchedPath, OnAssetFileChanged);
            asset.FullyReleasing += (s, e) => _pathWatcher.Unwatch(watchedPath, OnAssetFileChanged);

            async void OnAssetFileChanged(object s, FileChangedEventArgs e)
            {
                Logger.Info($"Asset changed: {assetPath} ({e.ChangeType})");
                await asset.ResetAsync();
            }

            return asset;
        }

        protected override T Load<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            if (CookAsset(assetPath, cancellationToken))
                return base.Load(loadingFunc, assetPath, cancellationToken);
            return default;
        }

        protected override async Task<T> LoadAsync<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            if (await CookAssetAsync(assetPath, cancellationToken))
                return await base.LoadAsync(loadingFunc, assetPath, cancellationToken);
            return default;
        }

        private bool CookAsset(string assetPath, CancellationToken cancellationToken)
        {
            bool foundImporter = false;
            Stopwatch stopwatch = null;
            try
            {
                _pipelineSemaphore.Wait(cancellationToken);

                stopwatch = Stopwatch.StartNew();
                try
                {
                    ICollection<string> rawFilePaths = GetRawFilesPaths(assetPath);
                    if (rawFilePaths.Count == 0)
                        return false; //throw new AssetNotFoundException(assetPath);

                    foreach (string rawFilePath in rawFilePaths)
                    {
                        string importerName = _pipelineManager.FindImporterByExtension(Path.GetExtension(rawFilePath));
                        if (importerName == null)
                            continue;

                        foundImporter = true;

                        string processorName = _pipelineManager.FindDefaultProcessor(importerName);
                        if (processorName == null)
                            continue;

                        string inputPath = Path.GetFullPath(rawFilePath);
                        string outputPath = Path.GetFullPath(Path.Combine(RootPath, rawFilePath.Substring(RawRootPath.Length + 1)));

                        try
                        {
                            _pipelineManager.BuildContent(inputPath, outputPath, importerName, processorName);
                        }
                        catch (PipelineException)
                        {
                            Logger.Info($"Failed to cook {assetPath}. Second try... (Importer: {importerName}, Processor: {processorName})");
                            _pipelineManager.BuildContent(inputPath, outputPath, importerName, processorName);
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        Logger.Info($"Cooked: {assetPath} ({stopwatch.ElapsedMilliseconds} ms, Importer: {importerName}, Processor: {processorName})");
                        return true;
                    }
                }
                finally
                {
                    stopwatch.Stop();
                    _pipelineSemaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Info($"Cancelled cooking: {assetPath} ({stopwatch?.ElapsedMilliseconds ?? 0} ms)");
                throw;
            }

            if (!foundImporter)
                throw new NoImporterException(assetPath);
            throw new NoProcessorException(assetPath);
        }

        private async Task<bool> CookAssetAsync(string assetPath, CancellationToken cancellationToken)
        {
            bool foundImporter = false;
            Stopwatch stopwatch = null;
            try
            {
                await _pipelineSemaphore.WaitAsync(cancellationToken);

                stopwatch = Stopwatch.StartNew();
                try
                {
                    ICollection<string> rawFilePaths = GetRawFilesPaths(assetPath);
                    if (rawFilePaths.Count == 0)
                        return false; //throw new AssetNotFoundException(assetPath);

                    foreach (string rawFilePath in rawFilePaths)
                    {
                        string importerName = _pipelineManager.FindImporterByExtension(Path.GetExtension(rawFilePath));
                        if (importerName == null)
                            continue;

                        foundImporter = true;

                        string processorName = _pipelineManager.FindDefaultProcessor(importerName);
                        if (processorName == null)
                            continue;

                        string inputPath = Path.GetFullPath(rawFilePath);
                        string outputPath = Path.GetFullPath(Path.Combine(RootPath, rawFilePath.Substring(RawRootPath.Length + 1)));

                        await Task.Run(
                            () =>
                            {
                                try
                                {
                                    return _pipelineManager.BuildContent(inputPath, outputPath, importerName, processorName);
                                }
                                catch (PipelineException)
                                {
                                    Logger.Info($"Failed to cook {assetPath}. Second try... (Importer: {importerName}, Processor: {processorName})");
                                    return _pipelineManager.BuildContent(inputPath, outputPath, importerName, processorName);
                                }
                            }, cancellationToken);

                        cancellationToken.ThrowIfCancellationRequested();

                        Logger.Info($"Cooked: {assetPath} ({stopwatch.ElapsedMilliseconds} ms, Importer: {importerName}, Processor: {processorName})");
                        return true;
                    }
                }
                finally
                {
                    stopwatch.Stop();
                    _pipelineSemaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Info($"Cancelled cooking: {assetPath} ({stopwatch?.ElapsedMilliseconds ?? 0} ms)");
                throw;
            }

            if (!foundImporter)
                throw new NoImporterException(assetPath);
            throw new NoProcessorException(assetPath);
        }

        private ICollection<string> GetRawFilesPaths(string assetPath)
        {
            string folderPath = Path.GetDirectoryName(assetPath);
            string fullFolderPath = folderPath != null ? Path.Combine(RawRootPath, folderPath) : RawRootPath;

            if (!Directory.Exists(fullFolderPath))
                return Array.Empty<string>();

            string assetName = Path.GetFileName(assetPath);
            return Directory.GetFiles(fullFolderPath, $"{assetName}.*");
        }

        public IEnumerable<string> GetSupportedFileExtensions(Type type)
        {
            if (typeof(IContentImporter).IsAssignableFrom(type))
                return GetSupportedExtensionsForImporter(type);
            if (typeof(IContentProcessor).IsAssignableFrom(type))
                return GetSupportedExtensionsForProcessor(type);
            return GetSupportedExtensionsForEngineContent(type);
        }

        private IEnumerable<string> GetSupportedExtensionsForImporter(Type importerType)
        {
            return importerType.GetCustomAttribute<ContentImporterAttribute>()?.FileExtensions ?? Enumerable.Empty<string>();
        }

        private IEnumerable<string> GetSupportedExtensionsForProcessor(Type processorType)
        {
            Type processorInputType = ProcessorsInputTypes[processorType.Name];

            return _pipelineManager.GetImporterTypes()
                .Where(x => x.BaseType?.GetGenericTypeDefinition() == typeof(ContentImporter<>))
                .Where(x => x.BaseType.GenericTypeArguments[0].IsAssignableFrom(processorInputType))
                .SelectMany(GetSupportedExtensionsForImporter);
        }

        private IEnumerable<string> GetSupportedExtensionsForEngineContent(Type contentType)
        {
            return _pipelineManager.GetProcessorTypes()
                .Where(x => ContentProcessorUtils.GetEngineContentType(x) != null
                    && contentType.IsAssignableFrom(ContentProcessorUtils.GetEngineContentType(x)))
                .SelectMany(GetSupportedExtensionsForProcessor);
        }

        private Dictionary<string, Type> _processorsInputTypes;
        private Dictionary<string, Type> ProcessorsInputTypes
        {
            get
            {
                return _processorsInputTypes ??
                    (_processorsInputTypes = _pipelineManager.GetProcessorTypes()
                        .ToDictionary(x => x.Name, x => _pipelineManager.CreateProcessor(x.Name, GetProcessorDefaultValues(x.Name)).InputType));
            }
        }

        private OpaqueDataDictionary GetProcessorDefaultValues(string processorName)
        {
            OpaqueDataDictionary processorDefaultValues = _pipelineManager.GetProcessorDefaultValues(processorName);
            foreach (string key in processorDefaultValues.Keys.ToArray())
            {
                if (processorDefaultValues[key] == null)
                    processorDefaultValues[key] = string.Empty;
            }

            return processorDefaultValues;
        }

        public Type GetContentType(string filePath)
        {
            string importerName = _pipelineManager.FindImporterByExtension(Path.GetExtension(filePath));
            if (importerName is null)
                return null;

            string processorName = _pipelineManager.FindDefaultProcessor(importerName);
            if (processorName is null)
                return null;

            Type processorType = _pipelineManager.GetProcessorType(processorName);
            if (processorType is null)
                return null;

            return ContentProcessorUtils.GetEngineContentType(processorType);
        }

        public void CleanCookedAssets()
        {
            if (Directory.Exists(IntermediateRootPath))
                Directory.Delete(IntermediateRootPath, recursive: true);
            if (Directory.Exists(RootPath))
                Directory.Delete(RootPath, recursive: true);
        }
    }
}