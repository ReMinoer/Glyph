using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawer
    {
        IDrawClient Client { get; }
        SpriteBatchStack SpriteBatchStack { get; }
        GraphicsDevice GraphicsDevice { get; }
        Resolution Resolution { get; }
        TopLeftRectangle ScreenBounds { get; }
        ICamera Camera { get; }
        RenderTarget2D DefaultRenderTarget { get; }
        Texture2D Output { get; }
        CenteredRectangle DisplayedRectangle { get; }
        Matrix ViewMatrix { get; }
        void ApplyEffects(IDrawer drawer);
    }
}