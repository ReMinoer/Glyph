using Glyph.Composition;

namespace Glyph.Game
{
    public interface IScene : IGlyphComposite, IEnableable, ILoadContent, IUpdate, IDraw
    {
        SceneNode RootNode { get; }
    }
}