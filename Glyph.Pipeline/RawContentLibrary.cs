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

        public string IntermediateRootPath { get; }
        public string RawRootPath { get; }
        protected override string WorkingDirectory => RawRootPath;

        public string FxCompilerPath { get; set; }
        public string FxProfile { get; set; } = "DirectX_11";

        public RawContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger, string rawRootPath, string cacheRootPath)
            : base(graphicsDeviceService, logger, Path.Combine(cacheRootPath, "bin"))
        {
            Directory.CreateDirectory(cacheRootPath);

            IntermediateRootPath = Path.Combine(cacheRootPath, "obj");
            RawRootPath = rawRootPath;

            _pathWatcher = new PathWatcher();

            // Song files are not valid if we are not using full paths
            _pipelineManager = new PipelineManager(Path.GetFullPath(RawRootPath), Path.GetFullPath(RootPath), Path.GetFullPath(IntermediateRootPath))
            {
                CompressContent = true,
                Profile = GraphicsProfile.HiDef,
                Platform = TargetPlatform.Windows
            };
            
            FxCompilerPath = ResolveFxCompilerPath();
        }

        static private string ResolveFxCompilerPath()
        {
            string ComputePath(Environment.SpecialFolder programFilesFolder)
            {
                return Path.Combine(Environment.GetFolderPath(programFilesFolder), @"MSBuild\\MonoGame\\v3.0\\Tools\\2MGFX.exe");
            }

            string path = ComputePath(Environment.SpecialFolder.Programs);
            if (File.Exists(path))
                return path;
            path = ComputePath(Environment.SpecialFolder.ProgramFilesX86);
            if (File.Exists(path))
                return path;
            path = ComputePath(Environment.SpecialFolder.ProgramFiles);
            if (File.Exists(path))
                return path;

            return null;
        }

        protected override IAsset<T> CreateAsset<T>(string assetPath, LoadDelegate<T> loadDelegate)
        {
            var asset = new Asset<T>(assetPath, loadDelegate);

            string watchedPath = Path.Combine(RawRootPath, assetPath + ".*");
            if (!PathUtils.IsValidAbsolutePath(watchedPath))
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

        protected override async Task<T> Load<T>(Func<ContentManager, string, T> loadingFunc, string assetPath, CancellationToken cancellationToken)
        {
            if (await CookAsset(assetPath, cancellationToken))
                return await base.Load(loadingFunc, assetPath, cancellationToken);
            return default;
        }

        protected override async Task<Effect> LoadEffect(string assetPath, CancellationToken cancellationToken)
        {
            if (await CompileEffect(assetPath, cancellationToken))
                return await base.LoadEffect(assetPath, cancellationToken);
            return null;
        }

        private async Task<bool> CookAsset(string assetPath, CancellationToken cancellationToken)
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

        private async Task<bool> CompileEffect(string assetPath, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                stopwatch = Stopwatch.StartNew();
                try
                {
                    string inputPath = Path.GetFullPath(Path.Combine(RawRootPath, assetPath + ".fx"));
                    if (!File.Exists(inputPath))
                        return false; //throw new AssetNotFoundException(assetPath);

                    string outputPath = Path.ChangeExtension(Path.GetFullPath(Path.Combine(RootPath, inputPath.Substring(RawRootPath.Length + 1))), ".mgfx");
                    string outputFolderPath = Path.GetDirectoryName(outputPath);
                    if (outputFolderPath != null)
                        Directory.CreateDirectory(outputFolderPath);

                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = FxCompilerPath,
                        Arguments = $"\"{inputPath}\" \"{outputPath}\" /Profile:{FxProfile}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true
                    };

                    var process = new Process
                    {
                        StartInfo = processStartInfo
                    };

                    await Task.Run(() =>
                    {
                        process.Start();
                        process.WaitForExit();
                    }, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (process.ExitCode != 0)
                    {
                        string rootPathToRemove = RawRootPath.Replace("\\", "\\\\") + "\\\\";

                        var errorMessages = new List<string>();
                        while (true)
                        {
                            string error = await process.StandardError.ReadLineAsync();
                            if (error == null)
                                break;
                            if (string.IsNullOrWhiteSpace(error))
                                continue;

                            errorMessages.Add(error.Replace(rootPathToRemove, ""));
                        }

                        Logger.Error($"Failed to compile {assetPath} (Profile: {FxProfile})");
                        foreach (string errorMessage in errorMessages)
                            Logger.Error(errorMessage);

                        return false;
                    }

                    Logger.Info($"Compiled: {assetPath} ({stopwatch.ElapsedMilliseconds} ms, Profile: {FxProfile})");
                    return true;
                }
                finally
                {
                    stopwatch.Stop();
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Info($"Cancelled compilation: {assetPath} ({stopwatch?.ElapsedMilliseconds ?? 0} ms)");
                throw;
            }
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

        private string GetAssetPathPattern(string rootPath, string assetPath)
        {
            string watchedPath = Path.Combine(rootPath, assetPath + ".*");
            if (!PathUtils.IsValidAbsolutePath(watchedPath))
                watchedPath = Path.Combine(Environment.CurrentDirectory, watchedPath);

            return watchedPath;
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

        public void CleanCookedAssets()
        {
            if (Directory.Exists(IntermediateRootPath))
                Directory.Delete(IntermediateRootPath, recursive: true);
            if (Directory.Exists(RootPath))
                Directory.Delete(RootPath, recursive: true);
        }
    }
}