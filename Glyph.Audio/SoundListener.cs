using Glyph.Composition;
using Microsoft.Xna.Framework.Audio;

namespace Glyph.Audio
{
    public class SoundListener : GlyphComponent
    {
        internal AudioListener AudioListener { get; private set; }

        public SoundListener(SoundListenerManager soundListenerManager)
        {
            AudioListener = new AudioListener();
            soundListenerManager.Add(this);
        }
    }
}