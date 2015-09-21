using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteLoader : GlyphComponent, ISpriteSource, ILoadContent
    {
        public string Asset { get; set; }
        public Texture2D Texture { get; set; }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            Texture = contentLibrary.GetTexture(Asset);
        }
    }
}