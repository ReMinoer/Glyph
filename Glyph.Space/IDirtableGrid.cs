using System.Collections.Generic;

namespace Glyph.Space
{
    public interface IDirtableGrid<T> : IWriteableGrid<T>, IDirtable
        where T : class, IDirtable
    {
        IEnumerable<IGridCase<T>> DirtiedCases { get; }
    }
}