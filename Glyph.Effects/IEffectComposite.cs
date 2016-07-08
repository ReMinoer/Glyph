using Stave;

namespace Glyph.Effects
{
    public interface IEffectComposite : IEffectComposite<IEffectComponent>
    {
    }

    public interface IEffectComposite<TComponent> : IEffectComponent, IComposite<IEffectComponent, IEffectParent, TComponent>
        where TComponent : class, IEffectComponent
    {
    }
}