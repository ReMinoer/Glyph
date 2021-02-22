using Glyph.Animation;
using Glyph.Composition;
using Glyph.Core;

namespace Glyph.Particles
{
    public interface IParticle : IDraw, ITimeOffsetable
    {
        SceneNode SceneNode { get; }
        bool Ended { get; }
    }
}