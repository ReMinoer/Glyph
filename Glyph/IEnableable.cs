namespace Glyph
{
    public interface IEnableable : IGlyphComponent
    {
        bool Enabled { get; }
    }
}