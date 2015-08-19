using Diese.Composition;

namespace Glyph.Effects
{
    public interface IEffectParent : IParent<IEffectComponent, IEffectParent>, IEffectComponent
    {
    }
}