using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Glyph.IO.Base;

namespace Glyph.IO
{
    public class DataContractSerializationFormat<T> : DataContractSerializationFormat, ISerializationFormat<T>
    {
        public void Save(T obj, Stream stream) => base.Save(obj, stream);
        new public T Load(Stream stream) => Load<T>(stream);
    }

    public class DataContractSerializationFormat : SerializationFormatBase
    {
        public override IEnumerable<Type> KnownTypes { get; set; }
        public Encoding Encoding { get; set; } = Encoding.Default;

        protected override object Load(Stream stream)
        {
            Type serializedType = ReadSerializedType(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var dataContractSerializer = new DataContractSerializer(serializedType, KnownTypes);
            return dataContractSerializer.ReadObject(stream);
        }

        private Type ReadSerializedType(Stream stream)
        {
            XName rootName = XDocument.Load(stream).Root?.Name;
            string loadedTypeName = rootName?.LocalName;
            string loadedNamespace = rootName?.NamespaceName.Split('/').Last();

            if (loadedTypeName == null || loadedNamespace == null)
                throw new SerializationException("Cannot retrieve serialized object type name !");

            string fullTypeName = $"{loadedNamespace}.{loadedTypeName}";
            Type serializedType = Type.GetType(fullTypeName) ?? KnownTypes?.FirstOrDefault(x => x.FullName == fullTypeName);
            if (serializedType == null)
                throw new SerializationException($"Cannot deserialize object of type \"{fullTypeName}\" !");

            return serializedType;
        }

        public override void Save(object obj, Stream stream)
        {
            using (var xmlTextWriter = new XmlTextWriter(stream, Encoding))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                new DataContractSerializer(obj.GetType(), KnownTypes).WriteObject(xmlTextWriter, obj);
            }
        }
    }
}