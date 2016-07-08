using Stave;

namespace Glyph.Effects
{
    public interface IEffectContainer : IEffectContainer<IEffectComponent>
    {
    }

    public interface IEffectContainer<out TComponent> : IEffectComponent, IContainer<IEffectComponent, IEffectParent, TComponent>
        where TComponent : class, IEffectComponent
    {
    }
}