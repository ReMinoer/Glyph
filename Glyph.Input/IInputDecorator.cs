using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputDecorator : IDecorator<IInputHandler>, IInputHandler
    {
    }

    public interface IInputDecorator<out TValue> : IInputDecorator, IInputHandler<TValue>
    {
    }
}