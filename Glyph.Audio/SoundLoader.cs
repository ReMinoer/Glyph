using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework.Audio;

namespace Glyph.Audio
{
    public class SoundLoader : GlyphComponent, ILoadContent
    {
        private readonly Dictionary<object, SoundEffect> _soundEffects;
        private readonly Dictionary<object, string> _assets;

        public SoundEffect this[object key]
        {
            get { return _soundEffects[key]; }
        }

        public SoundLoader()
        {
            _soundEffects = new Dictionary<object, SoundEffect>();
            _assets = new Dictionary<object, string>();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (KeyValuePair<object, string> pair in _assets)
            {
                object key = pair.Key;
                string asset = pair.Value;

                _soundEffects[key] = contentLibrary.GetSound(asset);
            }
        }

        public void Add(object key, string asset)
        {
            _assets.Add(key, asset);
        }

        public void Remove(object key)
        {
            _soundEffects.Remove(key);
            _assets.Remove(key);
        }
    }
}