using Glyph.Composition;
using NLog;

namespace Glyph.Tools
{
    static public class CompositionLog
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static public void Write(IGlyphComponent root, ISceneNode rootNode)
        {
            Logger.Debug($"Composition :\n\n{WriteComposition(root, 0)}\n\nScene graph :\n\n{WriteSceneGraph(rootNode, 0)}");
        }

        static private string WriteComposition(IGlyphComponent root, int level)
        {
            string result = "";

            for (int i = 0; i < level - 1; i++)
                result += "  ";
            if (level > 0)
                result += "|-";

            result += root.Name + "\n";

            foreach (IGlyphComponent component in root.GetAllComponents<IGlyphComponent>())
                result += WriteComposition(component, level + 1);

            return result;
        }

        static private string WriteSceneGraph(ISceneNode root, int level)
        {
            string result = "";

            for (int i = 0; i < level - 1; i++)
                result += "  ";
            if (level > 0)
                result += "|-";

            result += $"{root.Parent?.Name} (Local: T=[{root.LocalPosition}], R={root.LocalRotation}, S={root.LocalScale}, D={root.LocalDepth} - Global: T=[{root.Position}], R={root.Rotation}, S={root.Scale}, D={root.Depth})\n";

            foreach (ISceneNode component in root.Children)
                result += WriteSceneGraph(component, level + 1);

            return result;
        }
    }
}