using System;
using System.Threading.Tasks;
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

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            _texture = await contentLibrary.GetOrLoad<Texture2D>(Asset);

            Loaded?.Invoke(this);
        }
    }
}