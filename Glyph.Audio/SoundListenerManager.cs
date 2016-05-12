using System.Collections.Generic;

namespace Glyph.Audio
{
    public class SoundListenerManager
    {
        private readonly List<SoundListener> _soundListeners;

        public IEnumerable<SoundListener> SoundListeners
        {
            get { return _soundListeners.AsReadOnly(); }
        }

        public SoundListenerManager()
        {
            _soundListeners = new List<SoundListener>();
        }

        internal void Add(SoundListener soundListener)
        {
            _soundListeners.Add(soundListener);
        }
    }
}