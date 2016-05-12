namespace Glyph.Composition
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; set; }
        void Draw(IDrawer drawer);
    }
}