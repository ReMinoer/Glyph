using System;

namespace Glyph.Composition.Injection
{
    [Flags]
    public enum GlyphInjectableTargets
    {
        Parent = 1 << 0,
        Fraternal = 1 << 1,
        NewInstance = 1 << 2,
        All = Parent | Fraternal | NewInstance
    }
}