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

            IGlyphData[] bindedDataArray = Tree.BreadthFirst(root, x => x.Children).Where(x => x.BindedObject != null).ToArray();

            foreach (IGlyphData data in bindedDataArray)
                if (data.BindedObject == component)
                    return data;

            foreach (IGlyphContainer parent in component.ParentQueue())
                if (bindedDataArray.Any(x => x.BindedObject == parent, out IGlyphData data))
                    return data;

            return null;
        }
    }
}