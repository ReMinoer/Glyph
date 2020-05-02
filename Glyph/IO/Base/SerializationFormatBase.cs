using System;
using System.Collections.Generic;

namespace Glyph.IO.Base
{
    public abstract class SerializationFormatBase<T> : SaveLoadFormatBase<T>, ISerializationFormat<T>
    {
        public abstract IEnumerable<Type> KnownTypes { get; set; }
    }
}