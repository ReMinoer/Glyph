using System.IO;

namespace Glyph.IO
{
    public interface ISaveFormat<in T>
    {
        void Save(T obj, Stream stream);
    }
}