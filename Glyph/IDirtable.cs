using System;

namespace Glyph
{
    public interface IDirtable
    {
        event EventHandler Dirtied;
        event EventHandler DirtyCleaned;
        void SetDirty();
        void CleanDirty();
    }
}