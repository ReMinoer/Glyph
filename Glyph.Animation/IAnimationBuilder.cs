using Diese.Modelization;

namespace Glyph.Animation
{
    public interface IAnimationBuilder<T> : ICreator<IAnimation<T>>
    {
        bool Loop { get; set; }
    }
}