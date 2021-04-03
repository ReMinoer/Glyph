using Glyph.Math.Shapes;
using Glyph.Scheduling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawer
    {
        DrawScheduler DrawScheduler { get; }
        IDrawClient Client { get; }
        SpriteBatchStack SpriteBatchStack { get; }
        GraphicsDevice GraphicsDevice { get; }
        RenderTarget2D DefaultRenderTarget { get; }
        Quad DisplayedRectangle { get; }
        Vector2 ViewSize { get; }
        Matrix ViewMatrix { get; }
        Matrix SpriteBatchMatrix { get; }
        Matrix GetWorldViewProjectionMatrix(ISceneNode sceneNode);
        bool DrawPredicate(ISceneNode sceneNode);
        void Render();
        void RenderView();
    }
}