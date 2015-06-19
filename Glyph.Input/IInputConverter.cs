using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputConverter<out TInput, out TOutput> : ISynthesizer<IInputHandler, IInputParent, IInputHandler<TInput>>,
        IInputHandler<TOutput>
    {
    }
}