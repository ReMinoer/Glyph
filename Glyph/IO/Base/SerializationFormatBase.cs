using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Glyph.IO.Base
{
    public abstract class SerializationFormatBase : ISerializationFormat
    {
        public abstract IEnumerable<Type> KnownTypes { get; set; }

        protected abstract object Load(Stream stream);
        public abstract void Save(object obj, Stream stream);

        public T Load<T>(Stream stream)
        {
            object readObject = Load(stream);
            if (!(readObject is T result))
                throw new SerializationException($"Deserialized object of type \"{readObject.GetType().FullName}\" is not of the expected type \"{typeof(T).FullName}\" !");

            return result;
        }
    }
}