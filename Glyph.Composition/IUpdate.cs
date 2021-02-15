namespace Glyph.Composition
{
    public interface IUpdateTask
    {
        void Update(ElapsedTime elapsedTime);
    }

    public interface IUpdate : IGlyphComponent, IUpdateTask
    {
    }
}