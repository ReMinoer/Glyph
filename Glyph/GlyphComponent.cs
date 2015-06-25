using System;
using System.Collections.Generic;
using Diese.Composition;

namespace Glyph
{
    public class GlyphComponent : Component<IGlyphComponent, GlyphObject>, IGlyphComponent
    {
        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}