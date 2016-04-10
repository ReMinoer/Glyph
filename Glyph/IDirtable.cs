using System;

namespace Glyph
{
    public interface IDirtable
    {
        bool IsDirty { get; }
        event Action Dirtied;
    }
}