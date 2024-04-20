using System;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class EffectLoader : GlyphComponent, ILoadContent, IUpdate, IEffectSource
    {
        private readonly IContentLibrary _contentLibrary;
        private readonly AssetAsyncLoader<Effect> _assetAsyncLoader = new AssetAsyncLoader<Effect>();

        private string _assetPath;
        public string AssetPath
        {
            get => _assetPath;
            set
            {
                if (_assetPath == value)
                    return;

                _assetPath = value;
                _assetAsyncLoader.Asset = !string.IsNullOrWhiteSpace(value) ? _contentLibrary.GetAsset<Effect>(value) : null;
            }
        }

        private Effect _effect;
        public Effect Effect => _effect;

        public event Action<IEffectSource> EffectLoaded;

        public EffectLoader(IContentLibrary contentLibrary)
        {
            _contentLibrary = contentLibrary;
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            _assetAsyncLoader.LoadContent(CancellationToken.None);

            if (_assetAsyncLoader.UpdateContent(ref _effect))
                EffectLoaded?.Invoke(this);
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

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            await _assetAsyncLoader.LoadContentAsync(CancellationToken.None);

            if (_assetAsyncLoader.UpdateContent(ref _effect))
                EffectLoaded?.Invoke(this);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (_assetAsyncLoader.UpdateContent(ref _effect))
                EffectLoaded?.Invoke(this);
        }
    }
}