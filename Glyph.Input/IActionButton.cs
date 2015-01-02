namespace Glyph.Input
{
    public interface IActionButton
    {
        string Name { get; set; }

        bool IsPressed(InputManager input);

        bool IsDownNow(InputManager input);
    }
}