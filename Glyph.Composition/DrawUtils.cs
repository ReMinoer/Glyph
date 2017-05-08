namespace Glyph.Composition
{
    static public class DrawUtils
    {
        static public bool Displayed(this IDraw draw, IDrawClient drawClient)
        {
            return draw.Visible && (drawClient == null || draw.DrawClientFilter == null || draw.DrawClientFilter.Filter(drawClient));
        }
    }
}