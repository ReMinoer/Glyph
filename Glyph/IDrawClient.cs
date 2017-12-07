using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawClient
    {
        Matrix ResolutionMatrix { get; }
        GraphicsDevice GraphicsDevice { get; }
        RenderTarget2D DefaultRenderTarget { get; }
    }
}