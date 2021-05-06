using System.Linq;
using Diese.Collections;
using Glyph.Composition;
using Stave;

namespace Glyph.Core
{
    static public class SceneGraphUtils
    {
        static public SceneNode GetSceneNode(this IGlyphComponent component)
        {
            return component?.Components.FirstOfTypeOrDefault<SceneNode>()
                   ?? component?.AllParents().SelectMany(x => x.Components).FirstOfTypeOrDefault<SceneNode>();
        }
    }
}