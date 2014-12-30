using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Glyph.Xml
{
    public class XmlSaver
    {
        private readonly XmlWriter _writer;

        public XmlSaver(XmlWriter writer)
        {
            _writer = writer;
        }

        static public void Save<T>(string path, T x)
        {
            var stream = new StreamWriter(path);
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, x);
            stream.Close();
        }

        public void Write(string name, string value)
        {
            _writer.WriteElementString(name, value);
        }

        public void Write<T>(T value)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(_writer, value);
        }

        public void WriteList<T>(string name, List<T> value)
        {
            _writer.WriteStartElement(name);
            _writer.WriteAttributeString("count", value.Count.ToString(CultureInfo.InvariantCulture));

            if (value.Count != 0)
            {
                var serializer = new XmlSerializer(typeof(T));
                foreach (T x in value)
                    serializer.Serialize(_writer, x);
            }

            _writer.WriteEndElement();
        }

        public void WriteAttribute(string name, string value)
        {
            _writer.WriteAttributeString(name, value);
        }

        public void WriteAttribute<T>(string name, T value)
        {
            _writer.WriteAttributeString(name, value.ToString<T>());
        }

        public void WriteCollection(XmlCollection c)
        {
            foreach (KeyValuePair<string, string> x in c)
                _writer.WriteAttributeString(x.Key, x.Value);
        }
    }
}