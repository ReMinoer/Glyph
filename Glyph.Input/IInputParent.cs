using Diese.Composition;

namespace Glyph.Input
{
    public interface IInputParent : IParent<IInputHandler, IInputParent>, IInputHandler
    {
    }
}