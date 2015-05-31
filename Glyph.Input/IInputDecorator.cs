using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputDecorator<TComponent> : IDecorator<IInputHandler, TComponent>, IInputHandler
        where TComponent : IInputHandler
    {
    }

    public interface IInputDecorator<out TValue, TComponent> : IInputDecorator<TComponent>, IInputHandler<TValue>
        where TComponent : IInputHandler
    {
    }
}