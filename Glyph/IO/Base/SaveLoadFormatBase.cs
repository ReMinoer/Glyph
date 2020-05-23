using System.IO;

namespace Glyph.IO.Base
{
    public abstract class SaveLoadFormatBase<T> : ISaveLoadFormat<T>
    {
        public abstract T Load(Stream stream);
        public abstract void Save(T obj, Stream stream);
    }
}