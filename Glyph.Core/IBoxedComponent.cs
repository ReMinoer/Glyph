using Glyph.Composition;
using Glyph.Math;

namespace Glyph.Core
{
    public interface IBoxedComponent : IGlyphComponent
    {
        ISceneNode SceneNode { get; }
        IArea Area { get; }
    }
}