using System;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Threading;

namespace Glyph.Content
{
    public class Asset<T> : IAsset<T>
    {
        private readonly LoadDelegate<T> _loadDelegate;
        private readonly SemaphoreSlim _loadingSemaphore = new SemaphoreSlim(1);
        private Task<T> _loadingTask;
        private CancellationTokenSource _releaseCancellation;

        private int _handlerCounter;

        public string AssetPath { get; }

        public event EventHandler ContentChanged;
        public event EventHandler FullyReleasing;
        public event EventHandler FullyReleased;

        public Asset(string assetPath, LoadDelegate<T> loadDelegate)
        {
            AssetPath = assetPath;
            _loadDelegate = loadDelegate;
        }

        public ITask<T> GetContentAsync(CancellationToken cancellationToken) => new TaskWrapper<T>(GetContentAsyncInternal(cancellationToken));
        private async Task<T> GetContentAsyncInternal(CancellationToken cancellationToken)
        {
            await _loadingSemaphore.WaitAsync(cancellationToken);
            try
            {
                // Create loading task if it doesn't exists
                if (_loadingTask == null)
                {
                    _releaseCancellation = new CancellationTokenSource();
                    var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _releaseCancellation.Token);

                    _loadingTask = _loadDelegate(AssetPath, linkedCancellation.Token);
                }

                try
                {
                    // Await content
                    return await _loadingTask;
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
            }
        }

        public void Handle()
        {
            Interlocked.Increment(ref _handlerCounter);
        }

        public async Task<bool> ReleaseAsync()
        {
            if (Interlocked.Decrement(ref _handlerCounter) > 0)
                return false;

            FullyReleasing?.Invoke(this, EventArgs.Empty);
            await FullyReleaseAsync();
            FullyReleased?.Invoke(this, EventArgs.Empty);

            return true;
        }

        private async Task FullyReleaseAsync()
        {
            await StopLoadingAsync();
        }

        private async Task StopLoadingAsync()
        {
            await _loadingSemaphore.WaitAsync(CancellationToken.None);
            try
            {
                if (_loadingTask != null)
                {
                    // Cancel loading
                    _releaseCancellation?.Cancel();

                    try
                    {
                        // Await for loading end
                        await _loadingTask;
                    }
                    catch (OperationCanceledException)
                    {
                    }

                    // Dispose content if necessary
                    //if (_loadingTask.IsCompleted)
                    //    (_loadingTask.Result as IDisposable)?.Dispose();
                }

                _loadingTask = null;
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        public async Task ResetAsync()
        {
            await StopLoadingAsync();
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}