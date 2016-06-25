using Glyph.Composition;
using Glyph.Core;

namespace Glyph.Game
{
    public interface IScene : IGlyphComposite, IEnableable, ILoadContent, IUpdate, IDraw
    {
        SceneNode RootNode { get; }
    }
}