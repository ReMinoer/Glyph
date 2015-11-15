using Glyph.Composition;

namespace Glyph.Graphics
{
    public class SpriteLoader : SpriteSourceBase, ILoadContent
    {
        public string Asset { get; set; }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            Texture = contentLibrary.GetTexture(Asset);
        }
    }
}