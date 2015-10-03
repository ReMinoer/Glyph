using System.Linq;
using Glyph.Composition;
using Microsoft.Xna.Framework.Audio;

namespace Glyph.Audio
{
    // TASK : Add AudioSource component
    // TODO : Spatialisation du son
    public class SoundEmitter : GlyphComponent
    {
        private readonly SoundListenerManager _soundListenerManager;
        private SoundEffect _soundEffect;
        private SoundEffectInstance _soundEffectInstance;
        public AudioEmitter Model { get; private set; }
        public bool Spatialized { get; set; }

        public SoundEffect SoundEffect
        {
            get { return _soundEffect; }
            set
            {
                _soundEffect = value;
                _soundEffectInstance = _soundEffect.CreateInstance();

                _soundEffectInstance.Apply3D(
                    _soundListenerManager.SoundListeners.Select(x => x.Model).ToArray(),
                    Model);
            }
        }

        public SoundEmitter(SoundListenerManager soundListenerManager)
        {
            _soundListenerManager = soundListenerManager;

            Model = new AudioEmitter();
        }

        public void Play(string asset)
        {
            _soundEffectInstance.Play();
        }
    }
}