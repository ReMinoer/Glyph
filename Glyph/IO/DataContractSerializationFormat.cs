using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Glyph.IO.Base;

namespace Glyph.IO
{
    public class DataContractSerializationFormat<T> : SaveLoadFormatBase<T>, ISerializationFormat<T>
    {
        public override FileType FileType => new FileType
        {
            DisplayName = _displayName,
            Extensions = _fileExtensions
        };

        private readonly string _displayName;
        private readonly string[] _fileExtensions;

        public DataContractSerializationFormat()
        {
            _displayName = "XML";
            _fileExtensions = new[] { ".xml" };
        }

        public DataContractSerializationFormat(string displayName, IEnumerable<string> fileExtensions)
        {
            _displayName = displayName;
            _fileExtensions = fileExtensions.ToArray();
        }

        public DataContractSerializationFormat(string displayName, params string[] fileExtensions)
        {
            _displayName = displayName;
            _fileExtensions = fileExtensions;
        }

        public override T Load(Stream stream) => Load(stream, Enumerable.Empty<Type>());
        public override void Save(T obj, Stream stream) => Save(obj, stream, Enumerable.Empty<Type>());

        public T Load(Stream stream, IEnumerable<Type> knownTypes)
        {
            return (T)new DataContractSerializer(typeof(T), knownTypes).ReadObject(stream);
        }

        public void Save(T obj, Stream stream, IEnumerable<Type> knownTypes)
        {
            new DataContractSerializer(typeof(T), knownTypes).WriteObject(stream, obj);
        }
    }
}