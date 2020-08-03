using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework.Audio;

namespace Glyph.Audio
{
    public class SoundLoader : GlyphComponent, ILoadContent
    {
        private readonly ConcurrentDictionary<object, SoundEffect> _soundEffects;
        private readonly Dictionary<object, string> _assets;

        public SoundEffect this[object key] => _soundEffects[key];

        public SoundLoader()
        {
            _soundEffects = new ConcurrentDictionary<object, SoundEffect>();
            _assets = new Dictionary<object, string>();
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            await Task.WhenAll(_assets.Select(async x => _soundEffects[x.Key] = await contentLibrary.GetAsset<SoundEffect>(x.Value).GetContentAsync()));
        }

        public void Add(object key, string asset)
        {
            _assets.Add(key, asset);
        }

        public void Remove(object key)
        {
            _soundEffects.TryRemove(key, out SoundEffect _);
            _assets.Remove(key);
        }
    }
}