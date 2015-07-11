namespace Glyph.Composition
{
    public interface IEnableable : IGlyphComponent
    {
        bool Enabled { get; }
    }
}