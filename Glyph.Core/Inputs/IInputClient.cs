using Fingear.MonoGame;

namespace Glyph.Core.Inputs
{
    public interface IInputClient
    {
        Resolution Resolution { get; }
        IInputStates States { get; }
    }
}