using System.Linq;
using Glyph.Composition;
using Microsoft.Xna.Framework.Audio;

namespace Glyph.Audio
{
    public class SoundEmitter : GlyphComponent
    {
        private readonly SoundLoader _soundLoader;
        private readonly SoundListenerManager _soundListenerManager;
        private readonly AudioEmitter _audioEmitter;
        public bool Spatialized { get; set; }

        public SoundEmitter(SoundLoader soundLoader, SoundListenerManager soundListenerManager)
        {
            _soundLoader = soundLoader;
            _soundListenerManager = soundListenerManager;
            _audioEmitter = new AudioEmitter();
        }

        public void Play(object key, float volume = 1f, float pan = 0f, float pitch = 0f)
        {
            SoundEffectInstance soundEffectInstance = _soundLoader[key].CreateInstance();

            soundEffectInstance.Volume = volume;
            soundEffectInstance.Pan = pan;
            soundEffectInstance.Pitch = pitch;

            if (Spatialized)
                soundEffectInstance.Apply3D(_soundListenerManager.SoundListeners.Select(x => x.AudioListener).ToArray(), _audioEmitter);

            soundEffectInstance.Play();
        }
    }
}