using System;
using Glyph.Composition;

namespace Glyph.Graphics
{
    public class SpriteLoader : SpriteSourceBase, ILoadContent
    {
        public string Asset { get; set; }
        public override event Action<ISpriteSource> Loaded;

        public void LoadContent(ContentLibrary contentLibrary)
        {
            Texture = contentLibrary.GetTexture(Asset);

            if (Loaded != null)
                Loaded.Invoke(this);
        }
    }
}