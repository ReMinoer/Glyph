using Glyph.Math.Shapes;
using Glyph.Scheduling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawer
    {
        IDrawClient Client { get; }
        IDrawTask Root { get; }
        SpriteBatchStack SpriteBatchStack { get; }
        GraphicsDevice GraphicsDevice { get; }
        RenderTarget2D DefaultRenderTarget { get; }
        Quad DisplayedRectangle { get; }
        Vector2 ViewSize { get; }
        Matrix ViewMatrix { get; }
        bool DrawPredicate(ISceneNode sceneNode);
    }
}