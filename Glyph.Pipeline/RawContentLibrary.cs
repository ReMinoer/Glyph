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
using Simulacra.IO.Utils;
using Simulacra.IO.Watching;

namespace Glyph.Pipeline
{
    public class RawContentLibrary : ContentLibrary, IRawContentLibrary
    {
        private readonly PathWatcher _pathWatcher;
        private readonly PipelineManager _pipelineManager;
        private readonly SemaphoreSlim _pipelineSemaphore = new SemaphoreSlim(1);

        public string RawRootPath { get; }
        protected override string WorkingDirectory => RawRootPath;

        public RawContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger, string rawRootPath, string cacheRootPath)
            : base(graphicsDeviceService, logger, Path.Combine(cacheRootPath, "bin"))
        {
            Directory.CreateDirectory(cacheRootPath);

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
            foreach (string rawFilePath in GetRawFilesPaths(assetPath))
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
            if (await CookAsset(assetPath))
                return await base.Load(loadingFunc, assetPath, cancellationToken);
            return default;
        }

        protected override async Task<Effect> LoadEffect(string assetPath, CancellationToken cancellationToken)
        {
            if (await CopyEffect(assetPath))
                return await base.LoadEffect(assetPath, cancellationToken);
            return null;
        }

        private async Task<bool> CookAsset(string assetPath)
        {
            await _pipelineSemaphore.WaitAsync();
            Stopwatch stopwatch = Stopwatch.StartNew();

            bool foundImporter = false;
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

                    await Task.Run(() =>
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
                        }
                    );

                    Logger.Info($"Cooked {assetPath} ({stopwatch.ElapsedMilliseconds} ms, Importer: {importerName}, Processor: {processorName})");
                    return true;
                }
            }
            finally
            {
                stopwatch.Stop();
                _pipelineSemaphore.Release();
            }

            if (!foundImporter)
                throw new NoImporterException(assetPath);
            throw new NoProcessorException(assetPath);
        }

        private async Task<bool> CopyEffect(string assetPath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                string inputPath = Path.GetFullPath(Path.Combine(RawRootPath, assetPath + ".mgfx"));
                if (!File.Exists(inputPath))
                    return false; //throw new AssetNotFoundException(assetPath);

                string outputPath = Path.GetFullPath(Path.Combine(RootPath, inputPath.Substring(RawRootPath.Length + 1)));
                string outputFolderPath = Path.GetDirectoryName(outputPath);
                if (outputFolderPath != null)
                    Directory.CreateDirectory(outputFolderPath);

                await Task.Run(() => File.Copy(inputPath, outputPath, overwrite: true));

                Logger.Info($"Copied {assetPath} ({stopwatch.ElapsedMilliseconds} ms)");
                return true;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public ICollection<string> GetRawFilesPaths(string assetPath)
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
                .Where(x => contentType.IsAssignableFrom(ContentProcessorUtils.GetEngineContentType(x)))
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
    }
}