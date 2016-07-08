using Stave;

namespace Glyph.Input
{
    public interface IInputDecorator<TComponent> : IDecorator<IInputHandler, IInputParent, TComponent>, IInputHandler
        where TComponent : class, IInputHandler
    {
    }

    public interface IInputDecorator<out TValue, TComponent> : IInputDecorator<TComponent>, IInputHandler<TValue>
        where TComponent : class, IInputHandler
    {
    }
}