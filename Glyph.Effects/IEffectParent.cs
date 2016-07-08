using Stave;

namespace Glyph.Effects
{
    public interface IEffectParent : IParent<IEffectComponent, IEffectParent>, IEffectComponent
    {
    }
}