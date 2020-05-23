namespace Glyph.IO
{
    public interface ISaveLoadFormat<T> : ISaveFormat<T>, ILoadFormat<T>
    {
    }
}