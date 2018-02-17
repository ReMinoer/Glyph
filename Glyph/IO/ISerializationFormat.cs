using System;
using System.Collections.Generic;
using System.IO;

namespace Glyph.IO
{
    public interface ISerializationFormat<T> : ISaveLoadFormat<T>
    {
        T Load(Stream stream, IEnumerable<Type> knownTypes);
        void Save(T obj, Stream stream, IEnumerable<Type> knownTypes);
    }
}