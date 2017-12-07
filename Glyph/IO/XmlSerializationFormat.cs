using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Glyph.IO.Base;

namespace Glyph.IO
{
    public class XmlSerializationFormat<T> : SaveLoadFormatBase<T>
    {
        public override FileType FileType => new FileType
        {
            DisplayName = _displayName,
            Extensions = _fileExtensions
        };

        private readonly string _displayName;
        private readonly string[] _fileExtensions;

        public XmlSerializationFormat()
        {
            _displayName = "XML";
            _fileExtensions = new[] { ".xml" };
        }

        public XmlSerializationFormat(string displayName, IEnumerable<string> fileExtensions)
        {
            _displayName = displayName;
            _fileExtensions = fileExtensions.ToArray();
        }

        public XmlSerializationFormat(string displayName, params string[] fileExtensions)
        {
            _displayName = displayName;
            _fileExtensions = fileExtensions;
        }

        public override T Load(Stream stream)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
        }

        public override void Save(T obj, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, obj);
        }
    }
}