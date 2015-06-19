using Diese.Composition;

namespace Glyph.Effects
{
    public interface IEffectParent : IParent<IEffect, IEffectParent>, IEffect
    {
         
    }
}