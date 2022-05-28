using System.Linq;
using Diese.Collections;
using Stave;

namespace Glyph.Composition.Modelization
{
    static public class GlyphCreatorExtension
    {
        static public IGlyphData GetData(this IGlyphData root, IGlyphComponent component)
        {
            if (root == null || component == null)
                return null;

            foreach (IGlyphComponent parent in component.AndAllParents())
                if (Tree.BreadthFirst(root, x => x.Children.Concat(x.ChildrenSources.SelectMany(y => y.Children))).Any(x => x.BindedObject == parent, out IGlyphData data))
                    return data;

            return null;
        }
    }
}