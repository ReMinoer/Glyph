﻿using System;

namespace Glyph.Resolver
{
    [Flags]
    public enum ResolveTargets
    {
        Parent = 1 << 0,
        Fraternal = 1 << 1,
        BrowseAllAncestors = 1 << 2,
        Local = 1 << 3,
        Global = 1 << 4,
        All = Parent | Fraternal | BrowseAllAncestors | Local | Global
    }
}