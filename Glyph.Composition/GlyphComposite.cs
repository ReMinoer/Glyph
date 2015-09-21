﻿using System;
using Diese.Composition;

namespace Glyph.Composition
{
    public abstract class GlyphComposite : GlyphComposite<IGlyphComponent>
    {
    }

    public abstract class GlyphComposite<TComponent> : Composite<IGlyphComponent, IGlyphParent, TComponent>, IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            foreach (TComponent component in Components)
                component.Dispose();

            Clear();
        }
    }
}