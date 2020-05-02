using System;
using System.Collections.Generic;

namespace Glyph.IO
{
    public interface ISerializationFormat<T> : ISaveLoadFormat<T>
    {
        IEnumerable<Type> KnownTypes { get; set; }
    }
}