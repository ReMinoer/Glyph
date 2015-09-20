using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface ISpriteSource : IGlyphComponent
    {
         Texture2D Texture { get; }
    }
}