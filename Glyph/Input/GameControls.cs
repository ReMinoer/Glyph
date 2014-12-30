using System.IO;
using System.Xml.Serialization;
using Glyph.Xml;

namespace Glyph.Input
{
    public class GameControls : SerializableDictionary<string, IActionButton>
    {
        public void Add(ActionsCollection actions)
        {
            foreach (IActionButton action in actions)
                Add(action.Name, action);
        }

        public void SaveXml(string filename)
        {
            var file = new StreamWriter(filename);
            var serializer = new XmlSerializer(typeof(GameControls));
            serializer.Serialize(file, this);
            file.Close();
        }

        static public GameControls LoadXml(string filename)
        {
            var stream = new StreamReader(filename);
            var serializer = new XmlSerializer(typeof(GameControls));
            var controls = (GameControls)serializer.Deserialize(stream);

            stream.Close();
            return controls;
        }
    }
}