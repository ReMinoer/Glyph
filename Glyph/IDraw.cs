namespace Glyph
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; }
        void Draw();
    }
}