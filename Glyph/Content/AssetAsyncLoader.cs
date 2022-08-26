using System;
using System.Threading;
using System.Threading.Tasks;

namespace Glyph.Content
{
    public class AssetAsyncLoader<T> : IRestorable, IDisposable
        where T : class
    {
        private bool _stored;
        private bool _loaded;
        private bool _loadedOnce;

        private Task _loading;
        private CancellationTokenSource _loadingCancellation;
        private readonly SemaphoreSlim _loadingSemaphore = new SemaphoreSlim(1);

        private bool _needUpdate;
        private readonly object _contentLock = new object();
        private T _newContent;

        private IAsset<T> _asset;
        public IAsset<T> Asset
        {
            get => _asset;
            set
            {
                if (_asset == value)
                    return;

                UnloadAsset();
                _asset = value;
                LoadAsset();
            }
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            if (!_loadedOnce)
                return;

            // Start new loading
            _loadingCancellation?.Cancel();
            _loadingCancellation = new CancellationTokenSource();
            _loading = LoadingAsync(_loadingCancellation.Token);
        }

        public void LoadContent(CancellationToken cancellationToken)
        {
            _loadedOnce = true;

            _loadingSemaphore.Wait(cancellationToken);
            try
            {
                // Start new loading if none existing
                if (_loading is null)
                {
                    _loadingCancellation = new CancellationTokenSource();
                    var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(_loadingCancellation.Token, cancellationToken);

                    Loading(linkedCancellation.Token);
                    _loading = Task.CompletedTask;
                }
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        public async Task LoadContentAsync(CancellationToken cancellationToken)
        {
            _loadedOnce = true;

            await _loadingSemaphore.WaitAsync(cancellationToken);
            try
            {
                // Start new loading if none existing
                if (_loading is null)
                {
                    _loadingCancellation = new CancellationTokenSource();
                    var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(_loadingCancellation.Token, cancellationToken);

                    _loading = LoadingAsync(linkedCancellation.Token);
                }

                await _loading;
            }
            finally
            {
                _loadingSemaphore.Release();
            }
        }

        private void Loading(CancellationToken cancellationToken)
        {
            if (_asset == null)
                return;

            T content = _asset.GetContent(cancellationToken);

            // Throw if cancellation before assigning any new content
            cancellationToken.ThrowIfCancellationRequested();
            OnLoaded(content);
        }

        private async Task LoadingAsync(CancellationToken cancellationToken)
        {
            if (_asset == null)
                return;

            T content = await _asset.GetContentAsync(cancellationToken);

            // Throw if cancellation before assigning any new content
            cancellationToken.ThrowIfCancellationRequested();
            OnLoaded(content);
        }

        private void OnLoaded(T loadedContent)
        {
            lock (_contentLock)
            {
                _newContent = loadedContent;
                _needUpdate = true;
            }
        }

        public bool UpdateContent(ref T content)
        {
            lock (_contentLock)
            {
                if (!_needUpdate)
                    return false;

                _needUpdate = false;
                content = _newContent;
                return true;
            }
        }

        public void Store()
        {
            UnloadAsset();
            _stored = true;
        }

        public void Restore()
        {
            _stored = false;
            LoadAsset();
        }

        public void Dispose()
        {
            UnloadAsset();
        }

        private void LoadAsset()
        {
            if (_loaded)
                return;
            if (_stored)
                return;

            if (_asset is null)
            {
                // Need immediate update if new asset is null
                OnLoaded(null);
                return;
            }

            // Handle new asset and start loading only if it was already loaded previously
            _asset.Handle();

            if (_loadedOnce)
            {
                _loadingCancellation = new CancellationTokenSource();
                _loading = LoadingAsync(_loadingCancellation.Token);
            }

            _asset.ContentChanged += OnContentChanged;
            _loaded = true;
        }

        private void UnloadAsset()
        {
            if (!_loaded)
                return;
            if (_stored)
                return;
            if (_asset is null)
                return;
            
            // Cancel all loading and release previous asset
            _asset.ContentChanged -= OnContentChanged;

            _loading = null;
            _loadingCancellation?.Cancel();
            _loadingCancellation = null;

            _asset?.ReleaseAsync();
            _loaded = false;
        }
    }
}