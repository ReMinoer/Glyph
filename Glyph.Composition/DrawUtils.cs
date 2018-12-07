namespace Glyph.Composition
{
    static public class DrawUtils
    {
        static public bool Displayed(this IDraw draw, IDrawer drawer = null, IDrawClient drawClient = null, ISceneNode sceneRoot = null)
        {
            return draw.Visible
                && (drawer == null
                    || (sceneRoot == null || drawer.DrawPredicate(sceneRoot))
                        && (draw.DrawPredicate == null || draw.DrawPredicate(drawer)))
                && (drawClient == null || draw.DrawClientFilter == null || draw.DrawClientFilter.Filter(drawClient));
        }
    }
}