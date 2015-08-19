﻿using System;
using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphComposite : IGlyphEnumerable<IGlyphComponent>, IComposite<IGlyphComponent, IGlyphParent>
    {
        T Add<T>() where T : class, IGlyphComponent, new();
        IGlyphComponent Add(Type componentType);
    }
}