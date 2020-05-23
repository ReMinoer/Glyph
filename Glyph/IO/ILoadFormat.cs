using System.IO;

namespace Glyph.IO
{
    public interface ILoadFormat<out T>
    {
        T Load(Stream stream);
    }
}