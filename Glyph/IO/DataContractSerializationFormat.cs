using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Glyph.IO.Base;

namespace Glyph.IO
{
    public class DataContractSerializationFormat<T> : SerializationFormatBase<T>
    {
        public override FileType FileType => new FileType
        {
            DisplayName = _displayName,
            Extensions = _fileExtensions
        };

        private readonly string _displayName;
        private readonly string[] _fileExtensions;

        public override IEnumerable<Type> KnownTypes { get; set; }

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

        public override T Load(Stream stream)
        {
            Type serializedType = ReadSerializedType(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var dataContractSerializer = new DataContractSerializer(serializedType, KnownTypes);
            object readObject = dataContractSerializer.ReadObject(stream);
            if (!(readObject is T result))
                throw new SerializationException($"Deserialized object of type \"{readObject.GetType().FullName}\" is not of the expected type \"{serializedType.FullName}\" !");

            return result;
        }

        private Type ReadSerializedType(Stream stream)
        {
            XName rootName = XDocument.Load(stream).Root?.Name;
            string loadedTypeName = rootName?.LocalName;
            string loadedNamespace = rootName?.NamespaceName.Split('/').Last();

            if (loadedTypeName == null || loadedNamespace == null)
                throw new SerializationException("Cannot retrieve serialized object type name !");

            string fullTypeName = $"{loadedNamespace}.{loadedTypeName}";
            if (fullTypeName == typeof(T).FullName)
                return typeof(T);

            Type serializedType = Type.GetType(fullTypeName) ?? KnownTypes.FirstOrDefault(x => x.FullName == fullTypeName);
            if (serializedType == null)
                throw new SerializationException($"Cannot deserialize object of type \"{fullTypeName}\" !");

            return serializedType;
        }

        public override void Save(T obj, Stream stream)
        {
            new DataContractSerializer(obj.GetType(), KnownTypes).WriteObject(stream, obj);
        }
    }
}