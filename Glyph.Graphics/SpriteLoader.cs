using System;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteLoader : SpriteSourceBase, ILoadContent
    {
        public string Asset { get; set; }

        private Texture2D _texture;
        public override sealed Texture2D Texture => _texture;

        public override event Action<ISpriteSource> Loaded;

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            if (Asset == null)
            {
                _texture = null;
                return;
            }

            _texture = await contentLibrary.GetOrLoad<Texture2D>(Asset);
            Loaded?.Invoke(this);
        }
    }
}