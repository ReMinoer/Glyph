using System.Collections.Generic;
using Diese.Serialization;
using Glyph.Input.Handlers;

namespace Glyph.Input
{
    public class GameControls : SerializableDictionary<string, IInputHandler>
    {
        public void Add(IEnumerable<IInputHandler> inputHandlers)
        {
            foreach (IInputHandler inputHandler in inputHandlers)
                Add(inputHandler.Name, inputHandler);
        }

        public static GameControls Load(string path)
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