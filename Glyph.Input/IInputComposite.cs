using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputComposite : IInputHandler, IComposite<IInputHandler, IInputParent, IInputHandler>
    {
    }

    public interface IInputComposite<out TValue> : IInputComposite, IInputHandler<TValue>
    {
    }
}