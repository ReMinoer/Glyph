using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Stave;

namespace Glyph.Composition.Modelization
{
    static public class GlyphCreatorExtension
    {
        static public IGlyphCreator GetData(this IGlyphCreator root, IGlyphComponent component)
        {
            if (component == null)
                return null;

            IGlyphCreator[] bindedDataArray = Tree.BreadthFirst(root, x => x.Children).Where(x => x.BindedObject != null).ToArray();

            foreach (IGlyphCreator data in bindedDataArray)
                if (data.BindedObject == component)
                    return data;

            foreach (IGlyphContainer parent in component.ParentQueue())
                if (bindedDataArray.Any(x => x.BindedObject == parent, out IGlyphCreator data))
                    return data;

            return null;
        }

        static private class Tree
        {
            static public IEnumerable<T> BreadthFirst<T>(T root, Func<T, IEnumerable<T>> childrenSelector)
            {
                yield return root;

                foreach (T child in childrenSelector(root))
                    yield return child;
            }
        }
    }
}