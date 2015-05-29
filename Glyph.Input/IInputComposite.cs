using Diese.Composition.Composite;

namespace Glyph.Input
{
    public interface IInputComposite : IComposite<IInputHandler>, IInputHandler
    {
    }

    public interface IInputComposite<out TValue> : IInputComposite, IInputHandler<TValue>
    {
    }
}