using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Composition
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; set; }
        void Draw(SpriteBatch spriteBatch);
    }
}