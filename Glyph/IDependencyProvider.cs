namespace Glyph
{
    public interface IDependencyProvider
    {
        T Resolve<T>() where T : class, IGlyphComponent, new();
    }
}