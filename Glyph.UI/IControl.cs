using Glyph.Animation;
using Glyph.Composition;
using Glyph.Core;

namespace Glyph.UI
{
    public interface IControl : IEnableable, IDraw
    {
        SceneNode SceneNode { get; }
        Motion Motion { get; }
    }
}