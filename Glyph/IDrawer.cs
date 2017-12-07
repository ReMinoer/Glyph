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
        TopLeftRectangle ScreenBounds { get; }
        RenderTarget2D DefaultRenderTarget { get; }
        Texture2D Output { get; }
        CenteredRectangle DisplayedRectangle { get; }
        Matrix ViewMatrix { get; }
        Matrix ResolutionMatrix { get; }
        void ApplyEffects(IDrawer drawer);
    }
}