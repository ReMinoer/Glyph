namespace Glyph.Composition
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; }
        void Draw();
    }
}