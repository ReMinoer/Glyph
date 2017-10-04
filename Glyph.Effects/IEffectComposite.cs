using Stave;

namespace Glyph.Effects
{
    public interface IEffectComposite : IEffectComposite<IEffectComponent>
    {
    }

    public interface IEffectComposite<TComponent> : IEffectComponent, IComposite<IEffectComponent, IEffectContainer, TComponent>
        where TComponent : class, IEffectComponent
    {
    }
}