using System;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteLoader : SpriteSourceBase, ILoadContent
    {
        private Texture2D _texture;
        public string Asset { get; set; }
        public override event Action<ISpriteSource> Loaded;

        public override sealed Texture2D Texture
        {
            get { return _texture; }
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _texture = contentLibrary.GetTexture(Asset);

            if (Loaded != null)
                Loaded.Invoke(this);
        }
    }
}