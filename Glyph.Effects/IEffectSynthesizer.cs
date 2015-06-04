using Diese.Composition;

namespace Glyph.Effects
{
    public interface IEffectSynthesizer<out TInput> : IEffect, ISynthesizer<IEffect, TInput>
        where TInput : IEffect
    {
    }
}