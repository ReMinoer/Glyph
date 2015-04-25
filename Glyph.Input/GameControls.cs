using Diese.Serialization;

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
            var serializer = new SerializerXml<GameControls>();
            serializer.Save(this, filename);
        }

        static public GameControls LoadXml(string filename)
        {
            var serializer = new SerializerXml<GameControls>();
            return serializer.Instantiate(filename);
        }
    }
}