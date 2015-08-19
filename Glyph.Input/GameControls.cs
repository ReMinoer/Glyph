using System.Collections.Generic;
using Diese.Serialization;

namespace Glyph.Input
{
    public class GameControls : SerializableDictionary<object, IInputHandler>
    {
        public void Add(IEnumerable<IInputHandler> inputHandlers)
        {
            foreach (IInputHandler inputHandler in inputHandlers)
                Add(inputHandler.Name, inputHandler);
        }

        static public GameControls Load(string path)
        {
            var serializerXml = new SerializerXml<GameControls>();
            return serializerXml.Instantiate(path);
        }

        public void Save(string path)
        {
            var serializerXml = new SerializerXml<GameControls>();
            serializerXml.Save(this, path);
        }
    }
}