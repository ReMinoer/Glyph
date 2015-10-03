using Glyph.Animation;
using Glyph.Composition;

namespace Glyph.Graphics.Particles
{
    public interface IParticle : IUpdate, IDraw, ITimeOffsetable
    {
        bool Ended { get; }
    }
}