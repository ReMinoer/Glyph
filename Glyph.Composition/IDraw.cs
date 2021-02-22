using System;
using Diese.Collections;
using Glyph.Scheduling;

namespace Glyph.Composition
{
    public interface IDraw : IGlyphComponent, IDrawTask
    {
        bool Visible { get; set; }
        Predicate<IDrawer> DrawPredicate { get; set; }
        IFilter<IDrawClient> DrawClientFilter { get; set; }
    }
}