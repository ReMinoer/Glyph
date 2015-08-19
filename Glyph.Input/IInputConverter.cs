using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputConverter<out TInput, out TOutput> : IContainer<IInputHandler, IInputParent, IInputHandler<TInput>>,
        IInputHandler<TOutput>
    {
    }
}