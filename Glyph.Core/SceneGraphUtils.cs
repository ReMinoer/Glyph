using System;
using System.Collections.Generic;
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
            return component?.Components.FirstOrDefault<SceneNode>() ?? component?.ParentQueue().SelectMany(x => x.Components).FirstOrDefault<SceneNode>();
        }
    }
}