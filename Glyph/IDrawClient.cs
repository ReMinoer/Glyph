using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawClient
    {
        Resolution Resolution { get; }
        GraphicsDevice GraphicsDevice { get; }
        RenderTarget2D DefaultRenderTarget { get; }
    }
}