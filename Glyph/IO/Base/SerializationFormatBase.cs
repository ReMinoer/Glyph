using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Glyph.IO.Base
{
    public abstract class SerializationFormatBase<T> : SaveLoadFormatBase<T>, ISerializationFormat<T>
    {
        public override sealed T Load(Stream stream) => Load(stream, Enumerable.Empty<Type>());
        public override sealed void Save(T obj, Stream stream) => Save(obj, stream, Enumerable.Empty<Type>());

        public abstract T Load(Stream stream, IEnumerable<Type> knownTypes);
        public abstract void Save(T obj, Stream stream, IEnumerable<Type> knownTypes);
    }
}