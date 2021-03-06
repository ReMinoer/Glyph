﻿using Glyph.Scheduling;

namespace Glyph
{
    static public class DrawUtils
    {
        static public bool Displayed(this IDrawTask draw, IDrawer drawer = null, IDrawClient drawClient = null, ISceneNode sceneRoot = null)
        {
            return draw.Rendered
                && (drawer == null
                    || (sceneRoot == null || drawer.DrawPredicate(sceneRoot))
                        && (draw.DrawPredicate == null || draw.DrawPredicate(drawer)))
                && (drawClient == null || draw.DrawClientFilter == null || draw.DrawClientFilter.Filter(drawClient));
        }
    }
}