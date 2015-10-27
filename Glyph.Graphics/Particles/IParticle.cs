using Glyph.Animation;
using Glyph.Composition;

namespace Glyph.Graphics.Particles
{
    public interface IParticle : IUpdate, IDraw, ITimeOffsetable
    {
        SceneNode SceneNode { get; }
        bool Ended { get; }
    }
}