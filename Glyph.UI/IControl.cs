using Glyph.Animation;
using Glyph.Composition;

namespace Glyph.UI
{
    public interface IControl : IDraw
    {
        SceneNode SceneNode { get; }
        Motion Motion { get; }
        Shadow? Shadow { get; set; }
    }
}