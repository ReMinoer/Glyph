using System.IO;

namespace Glyph.IO
{
    public interface ILoadFormat<out T>
    {
        FileType FileType { get; }
        T Load(Stream stream);
    }
}