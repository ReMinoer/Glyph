namespace Glyph.Composition
{
    static public class DrawUtils
    {
        static public bool Displayed(this IDraw draw, IDrawClient drawClient = null, IDrawer drawer = null)
        {
            return draw.Visible
                && (drawer == null || draw.DrawPredicate == null || draw.DrawPredicate(drawer))
                && (drawClient == null || draw.DrawClientFilter == null || draw.DrawClientFilter.Filter(drawClient));
        }
    }
}