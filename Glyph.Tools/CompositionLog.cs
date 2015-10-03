using Glyph.Composition;
using NLog;

namespace Glyph.Tools
{
    static public class CompositionLog
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static public void Write(IGlyphComponent root)
        {
            Logger.Debug(string.Format("Composition :\n\n{0}", Write(root, 0)));
        }

        static private string Write(IGlyphComponent root, int level)
        {
            string result = "";

            for (int i = 0; i < level - 1; i++)
                result += "  ";
            if (level > 0)
                result += "|-";

            result += root.Name + "\n";

            foreach (IGlyphComponent component in root.GetAllComponents<IGlyphComponent>())
                result += Write(component, level + 1);

            return result;
        }
    }
}