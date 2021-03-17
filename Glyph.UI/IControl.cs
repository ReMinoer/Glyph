using Glyph.Animation;
using Glyph.Core;

namespace Glyph.UI
{
    public interface IControl
    {
        SceneNode SceneNode { get; }
        Motion Motion { get; }
    }
}