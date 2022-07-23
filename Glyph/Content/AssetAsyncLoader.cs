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

        public Task LoadContent(CancellationToken cancellationToken)
        {
            _loadedOnce = true;

            // Return existing loading task
            if (_loading != null)
                return _loading;

            // Start new loading if none existing
            _loadingCancellation = new CancellationTokenSource();
            var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(_loadingCancellation.Token, cancellationToken);
            return _loading = LoadingAsync(linkedCancellation.Token);
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