namespace Glyph
{
    public interface IUpdate : IGlyphComponent
    {
        void Update(ElapsedTime elapsedTime);
    }
}