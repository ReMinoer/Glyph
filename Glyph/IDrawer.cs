using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawer
    {
        SpriteBatchStack SpriteBatchStack { get; }
        GraphicsDevice GraphicsDevice { get; }
        IRectangle ScreenBounds { get; }
        ICamera Camera { get; }
        Texture2D Output { get; }
        IRectangle DisplayedRectangle { get; }
        Matrix ViewMatrix { get; }
        void ApplyEffects(IDrawer drawer);
    }
}