
using Diese.Collections;

namespace Glyph.Composition
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; set; }
        IFilter<IDrawClient> DrawClientFilter { get; set; }
        void Draw(IDrawer drawer);
    }
}