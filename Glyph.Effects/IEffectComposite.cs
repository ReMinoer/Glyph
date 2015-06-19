using Diese.Composition;

namespace Glyph.Effects
{
    public interface IEffectComposite : IEffect, IComposite<IEffect, IEffectParent>
    {
    }
}