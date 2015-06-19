using Diese.Composition;

namespace Glyph.Effects
{
    public interface IEffectSynthesizer<out TInput> : IEffect, ISynthesizer<IEffect, IEffectParent, TInput>
        where TInput : IEffect
    {
    }
}