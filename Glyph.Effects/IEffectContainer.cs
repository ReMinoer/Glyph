using Stave;

namespace Glyph.Effects
{
    public interface IEffectContainer : IContainer<IEffectComponent, IEffectContainer>, IEffectComponent
    {
    }

    public interface IEffectContainer<TComponent> : IEffectContainer, IContainer<IEffectComponent, IEffectContainer, TComponent>
        where TComponent : class, IEffectComponent
    {
    }
}