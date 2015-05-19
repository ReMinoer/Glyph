namespace Glyph.Input
{
    public interface IInputDecorator : IInputHandler
    {
        IInputHandler Component { get; set; }
    }

    public interface IInputDecorator<out TValue> : IInputDecorator, IInputHandler<TValue>
    {
    }
}