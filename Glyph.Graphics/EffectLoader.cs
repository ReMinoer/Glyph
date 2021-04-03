using System;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class EffectLoader : GlyphComponent, ILoadContent, IUpdate
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
                _assetAsyncLoader.Asset = !string.IsNullOrWhiteSpace(value) ? _contentLibrary.GetEffectAsset(value) : null;
            }
        }

        private Effect _effect;
        public Effect Effect => _effect;

        public event Action<Effect> Loaded;

        public EffectLoader(IContentLibrary contentLibrary)
        {
            _contentLibrary = contentLibrary;
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            await _assetAsyncLoader.LoadContent(CancellationToken.None);

            if (_assetAsyncLoader.UpdateContent(ref _effect))
                Loaded?.Invoke(_effect);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (_assetAsyncLoader.UpdateContent(ref _effect))
                Loaded?.Invoke(_effect);
        }
    }
}