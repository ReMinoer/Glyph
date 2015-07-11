namespace Glyph.Composition
{
    public interface IUpdate : IGlyphComponent
    {
        void Update(ElapsedTime elapsedTime);
    }
}