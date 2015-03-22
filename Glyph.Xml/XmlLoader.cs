using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Glyph.Xml
{
    public class XmlLoader
    {
        private readonly XmlReader _reader;
        private bool _isAttributesRead;

        public XmlLoader(XmlReader reader)
        {
            _reader = reader;
            _isAttributesRead = false;
        }

        static public T Load<T>(string path)
        {
            var stream = new StreamReader(path);
            var serializer = new XmlSerializer(typeof(T));
            var result = (T)serializer.Deserialize(stream);
            stream.Close();
            return result;
        }

        public void End()
        {
            if (_isAttributesRead)
                _reader.ReadEndElement();
            else
                _reader.ReadStartElement();
        }

        public string Read(string name)
        {
            if (!_isAttributesRead)
                EndOfAttributesReading();

            return _reader.ReadElementString(name);
        }

        public T Read<T>()
        {
            if (!_isAttributesRead)
                EndOfAttributesReading();

            var serializer = new XmlSerializer(typeof(T));
            var result = (T)serializer.Deserialize(_reader);
            return result;
        }

        public List<T> ReadList<T>(string name)
        {
            if (!_isAttributesRead)
                EndOfAttributesReading();

            var list = new List<T>();

            _reader.MoveToAttribute("count");
            int count = int.Parse(_reader.Value);
            _reader.ReadStartElement(name);

            var serializer = new XmlSerializer(typeof(T));
            for (var i = 0; i < count; i++)
                list.Add((T)serializer.Deserialize(_reader));

            _reader.ReadEndElement();
            return list;
        }

        public string ReadAttribute(string name)
        {
            return _reader[name];
        }

        public T ReadAttribute<T>(string name)
        {
            return _reader[name].Parse<T>();
        }

        public XmlCollection ReadCollection()
        {
            var c = new XmlCollection();

            while (_reader.MoveToNextAttribute())
                c.Add(_reader.Name, _reader.Value);

            return c;
        }

        private void EndOfAttributesReading()
        {
            _reader.ReadStartElement();
            _isAttributesRead = true;
        }
    }
}