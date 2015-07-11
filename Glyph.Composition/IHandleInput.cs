using Glyph.Input;

namespace Glyph.Composition
{
    public interface IHandleInput : IGlyphComponent
    {
        void HandleInput(InputManager inputManager);
    }
}