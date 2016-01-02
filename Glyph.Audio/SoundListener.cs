using Glyph.Composition;
using Microsoft.Xna.Framework.Audio;

namespace Glyph.Audio
{
    public class SoundListener : GlyphComponent
    {
        public AudioListener Model { get; private set; }

        public SoundListener(SoundListenerManager soundListenerManager)
        {
            Model = new AudioListener();

            soundListenerManager.Add(this);
        }
    }
}