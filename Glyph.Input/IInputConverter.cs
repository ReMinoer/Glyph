namespace Glyph.Input
{
    public interface IInputConverter<out TInput, out TOutput> : IInputHandler<TOutput>
    {
        IInputHandler<TInput>[] Components { get; }
    }
}