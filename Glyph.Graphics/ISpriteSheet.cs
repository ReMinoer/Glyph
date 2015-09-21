using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface ISpriteSheet : ILoadContent
    {
        int Count { get; }
        ISpriteSheetCarver Carver { get; }
        Rectangle this[int index] { get; }
        Texture2D GetFrameTexture(int frameIndex);
        void ApplyCarver(ISpriteSheetCarver carver);
    }
}