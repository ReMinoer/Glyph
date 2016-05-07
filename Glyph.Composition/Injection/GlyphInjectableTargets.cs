using System;

namespace Glyph.Composition.Injection
{
    [Flags]
    public enum GlyphInjectableTargets
    {
        Parent = 1 << 0,
        Fraternal = 1 << 1,
        Local = 1 << 2,
        Global = 1 << 3,
        All = Parent | Fraternal | Local | Global
    }
}