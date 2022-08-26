using System;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteLoader : SpriteSourceBase, ILoadContent, IUpdate
    {
        private readonly IContentLibrary _contentLibrary;

        private Texture2D _texture;
        public override sealed Texture2D Texture => _texture;

        private string _assetPath;
        private readonly AssetAsyncLoader<Texture2D> _assetAsyncLoader = new AssetAsyncLoader<Texture2D>();

        public string AssetPath
        {
            get => _assetPath;
            set
            {
                if (_assetPath == value)
                    return;

                _assetPath = value;
                _assetAsyncLoader.Asset = !string.IsNullOrWhiteSpace(value) ? _contentLibrary.GetAsset<Texture2D>(value) : null;
            }
        }

        public override event Action<ISpriteSource> Loaded;

        public SpriteLoader(IContentLibrary contentLibrary)
        {
            _contentLibrary = contentLibrary;
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            _assetAsyncLoader.LoadContent(CancellationToken.None);

            if (_assetAsyncLoader.UpdateContent(ref _texture))
                Loaded?.Invoke(this);
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            await _assetAsyncLoader.LoadContentAsync(CancellationToken.None);

            if (_assetAsyncLoader.UpdateContent(ref _texture))
                Loaded?.Invoke(this);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (_assetAsyncLoader.UpdateContent(ref _texture))
                Loaded?.Invoke(this);
        }

        public override void Store()
        {
            _assetAsyncLoader.Store();
            base.Store();
        }

        public override void Restore()
        {
            base.Restore();
            _assetAsyncLoader.Restore();
        }

        public override void Dispose()
        {
            _assetAsyncLoader.Dispose();
            base.Dispose();
        }
    }
}