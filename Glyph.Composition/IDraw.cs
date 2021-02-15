using System;
using Diese.Collections;

namespace Glyph.Composition
{
    public interface IDrawTask
    {
        void Draw(IDrawer drawer);
    }

    public interface IDraw : IGlyphComponent, IDrawTask
    {
        bool Visible { get; set; }
        Predicate<IDrawer> DrawPredicate { get; set; }
        IFilter<IDrawClient> DrawClientFilter { get; set; }
    }
}