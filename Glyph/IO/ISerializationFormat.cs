using System;
using System.Collections.Generic;
using System.IO;

namespace Glyph.IO
{
    public interface ISerializationFormat
    {
        IEnumerable<Type> KnownTypes { get; set; }
        T Load<T>(Stream stream);
        void Save(object obj, Stream stream);
    }

    public interface ISerializationFormat<T> : ISerializationFormat, ISaveLoadFormat<T>
    {
    }
}