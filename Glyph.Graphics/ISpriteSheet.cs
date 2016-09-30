using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface ISpriteSheet : ISpriteSource
    {
        int FramesCount { get; }
        ISpriteSheetCarver Carver { get; }
        int CurrentFrame { get; set; }
        Rectangle GetFrameRectangle(int frameIndex);
        Texture2D GetFrameTexture(int frameIndex);
        void ApplyCarver(ISpriteSheetCarver carver);
    }
}