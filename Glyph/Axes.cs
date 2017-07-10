using System;

namespace Glyph
{
    [Flags]
    public enum Axes
    {
        None = 0,
        Horizontal = 1 << 0,
        Vertical = 1 << 1,
        Both = Horizontal | Vertical
    }
}