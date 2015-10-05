using Glyph.Animation;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Game
{
    public interface IScene : IGlyphComposite, IEnableable, ILoadContent, IUpdate, IDraw
    {
        SceneNode RootNode { get; }
        void PreDraw(SpriteBatch spriteBatch);
        void PostDraw();
    }
}