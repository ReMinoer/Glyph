using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Glyph
{
    public class SoundCollection
    {
        private readonly Period _period;
        private readonly List<SoundEffect> _sounds;
        private int _id;

        public SoundCollection(int inter)
        {
            _sounds = new List<SoundEffect>();
            _period = new Period(inter);
            _id = 0;
        }

        public void AddSound(SoundEffect s)
        {
            _sounds.Add(s);
        }

        public void Update(GameTime gameTime)
        {
            _period.Update(gameTime);

            if (!_period.IsEnd)
                return;

            _period.Init();
            Play();
        }

        public void Play()
        {
            _id++;
            if (_id >= _sounds.Count)
                _id = 0;

            _sounds[_id].Play();
        }
    }
}