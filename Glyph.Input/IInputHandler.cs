using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputHandler : IComponent<IInputHandler, IInputParent>
    {
        bool IsActivated { get; }
        InputSource InputSource { get; }
        void Update(InputStates inputStates);
    }

    public interface IInputHandler<out TValue> : IInputHandler
    {
        TValue Value { get; }
    }
}