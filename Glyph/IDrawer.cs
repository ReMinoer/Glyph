using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawer
    {
        SpriteBatchStack SpriteBatchStack { get; }
        GraphicsDevice GraphicsDevice { get; }
    }
}