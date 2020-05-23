using System;
using System.Collections.Generic;
using System.IO;

namespace Glyph.IO
{
    static public class SerializationFormatExtension
    {
        static public ISerializationFormat<T> AsGeneric<T>(this ISerializationFormat serializationFormat) => new GenericSerializationFormat<T>(serializationFormat);

        public class GenericSerializationFormat<T> : ISerializationFormat<T>
        {
            private readonly ISerializationFormat _serializationFormat;
            public IEnumerable<Type> KnownTypes { get; set; }

            public GenericSerializationFormat(ISerializationFormat serializationFormat)
            {
                _serializationFormat = serializationFormat;
            }

            public void Save(T obj, Stream stream) => _serializationFormat.Save(obj, stream);
            public T Load(Stream stream) => _serializationFormat.Load<T>(stream);

            TLoaded ISerializationFormat.Load<TLoaded>(Stream stream) => _serializationFormat.Load<TLoaded>(stream);
            void ISerializationFormat.Save(object obj, Stream stream) => _serializationFormat.Save(obj, stream);
        }
    }
}