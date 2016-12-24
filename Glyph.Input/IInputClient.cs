using Fingear.MonoGame;

namespace Glyph.Input
{
    public interface IInputClient
    {
        Resolution Resolution { get; }
        IInputStates States { get; }
    }
}