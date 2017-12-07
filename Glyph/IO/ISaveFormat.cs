using System.IO;

namespace Glyph.IO
{
    public interface ISaveFormat<in T>
    {
        FileType FileType { get; }
        void Save(T obj, Stream stream);
    }
}