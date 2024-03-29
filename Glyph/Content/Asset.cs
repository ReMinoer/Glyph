using System;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Threading;

namespace Glyph.Content
{
    public class Asset<T> : IAsset<T>
    {
        static private readonly SemaphoreSlim _firstAssetSemaphore = new SemaphoreSlim(1);
        static private bool _isFirstAsset = true;

        private readonly LoadAsyncDelegate<T> _loadAsyncDelegate;
        private readonly LoadDelegate<T> _loadDelegate;
        private readonly SemaphoreSlim _loadingSemaphore = new SemaphoreSlim(1);
        private Task<T> _loadingTask;
        private CancellationTokenSource _releaseCancellation;

        private int _handlerCounter;

        public string AssetPath { get; }

        public event EventHandler ContentChanged;
        public event EventHandler FullyReleasing;
        public event EventHandler FullyReleased;

        public Asset(string assetPath, LoadAsyncDelegate<T> loadAsyncDelegate, LoadDelegate<T> loadDelegate)
        {
            AssetPath = assetPath;
            _loadAsyncDelegate = loadAsyncDelegate;
            _loadDelegate = loadDelegate;
        }

        public T GetContent(CancellationToken cancellationToken)
        {
            // Some asset init singleton at first instance (OpenAL). We will lock the first asset loading.
            _firstAssetSemaphore.Wait(cancellationToken);

            bool isFirstAsset = _isFirstAsset;
            if (!isFirstAsset)
                _firstAssetSemaphore.Release();

            _loadingSemaphore.Wait(cancellationToken);

            try
            {
                // Create loading task if it doesn't exists
                if (_loadingTask == null)
                {
                    _releaseCancellation = new CancellationTokenSource();
                    var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _releaseCancellation.Token);

                    _loadingTask = Task.FromResult(_loadDelegate(AssetPath, linkedCancellation.Token));
                }

                try
                {
                    // Await content
                    T asset = _loadingTask.Result;
                    _isFirstAsset = false;
                    return asset;
                }
                catch (OperationCanceledException)
                {
                    // Rethrow only if cancellation came from user. Return default if it came from release.
                    if (cancellationToken.IsCancellationRequested)
                        throw;
                    return default;
                }
            }
            finally
            {
                _loadingSemaphore.Release();

                if (isFirstAsset)
                    _firstAssetSemaphore.Release();
            }
        }

        public ITask<T> GetContentAsync(CancellationToken cancellationToken) => new TaskWrapper<T>(GetContentAsyncInternal(cancellationToken));
        private async Task<T> GetContentAsyncInternal(CancellationToken cancellationToken)
        {
            // Some asset init singleton at first instance (OpenAL). We will lock the first asset loading.
            await _firstAssetSemaphore.WaitAsync(cancellationToken);

            bool isFirstAsset = _isFirstAsset;
            if (!isFirstAsset)
                _firstAssetSemaphore.Release();

            await _loadingSemaphore.WaitAsync(cancellationToken);

            try
            {
                // Create loading task if it doesn't exists
                if (_loadingTask == null)
                {
                    _releaseCancellation = new CancellationTokenSource();
                    var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _releaseCancellation.Token);

                    _loadingTask = _loadAsyncDelegate(AssetPath, linkedCancellation.Token);
                }

                try
                {
                    // Await content
                    T asset = await _loadingTask;
                    _isFirstAsset = false;
                    return asset;
                }
                catch (OperationCanceledException)
                {
                    // Rethrow only if cancellation came from user. Return default if it came from release.
                    if (cancellationToken.IsCancellationRequested)
                        throw;
                    return default;
                }
            }
            finally
            {
                _loadingSemaphore.Release();

                if (isFirstAsset)
                    _firstAssetSemaphore.Release();
            }
        }

        public void Handle()
        {
            Interlocked.Increment(ref _handlerCounter);
        }

        public bool Release()
        {
            if (Interlocked.Decrement(ref _handlerCounter) > 0)
                return false;

            _loadingSemaphore.Wait(CancellationToken.None);
            try
            {
                FullyReleasing?.Invoke(this, EventArgs.Empty);
                StopLoading();
                FullyReleased?.Invoke(this, EventArgs.Empty);
                return true;
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        public async Task<bool> ReleaseAsync()
        {
            if (Interlocked.Decrement(ref _handlerCounter) > 0)
                return false;

            await _loadingSemaphore.WaitAsync(CancellationToken.None);
            try
            {
                FullyReleasing?.Invoke(this, EventArgs.Empty);
                await StopLoadingAsync();
                FullyReleased?.Invoke(this, EventArgs.Empty);
                return true;
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        public async Task ResetAsync()
        {
            await _loadingSemaphore.WaitAsync(CancellationToken.None);
            try
            {
                await StopLoadingAsync();
                ContentChanged?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        private void StopLoading()
        {
            if (_loadingTask == null)
                return;

            // Cancel loading
            _releaseCancellation?.Cancel();

            try
            {
                _loadingTask.Wait();
            }
            catch (OperationCanceledException)
            {
            }

            // Dispose content if necessary
            //if (_loadingTask.IsCompleted)
            //    (_loadingTask.Result as IDisposable)?.Dispose();

            _loadingTask = null;
        }

        private async Task StopLoadingAsync()
        {
            if (_loadingTask == null)
                return;

            // Cancel loading
            _releaseCancellation?.Cancel();

            try
            {
                await _loadingTask;
            }
            catch (OperationCanceledException)
            {
            }

            // Dispose content if necessary
            //if (_loadingTask.IsCompleted)
            //    (_loadingTask.Result as IDisposable)?.Dispose();

            _loadingTask = null;
        }
    }
}