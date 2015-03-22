using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class Texture2DSplit
    {
        private readonly List<Texture2D> _textures;

        public int Count
        {
            get { return _textures.Count; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public Texture2DSplit()
        {
            _textures = new List<Texture2D>();
        }

        public Texture2D this[int i]
        {
            get { return _textures[i]; }
        }

        public void LoadContent(ContentLibrary ressources, string asset)
        {
            _textures.Clear();
            if (asset == "")
                return;

            _textures.Add(ressources.GetTexture(asset));
            for (var i = 2; ressources.Contains(asset + i); i++)
                _textures.Add(ressources.GetTexture(asset + i));
        }
    }
}