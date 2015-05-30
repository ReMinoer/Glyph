using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputConverter<out TInput, out TOutput> : ISynthesizer<IInputHandler, IInputHandler<TInput>>,
        IInputHandler<TOutput>
    {
    }
}