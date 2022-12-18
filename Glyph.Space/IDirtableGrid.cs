using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Space
{
    public interface IDirtableGrid<T> : IWriteableGrid<T>, IDirtable
        where T : class, IDirtableGridCell<T>
    {
        IEnumerable<IGridCase<T>> DirtiedCases { get; }
        event EventHandler<CellsDirtiedEventArgs<T>> CellsDirtied;
        void SetDirty(T value);
    }

    public class CellsDirtiedEventArgs<T> : EventArgs
        where T : class, IDirtableGridCell<T>
    {
        public IReadOnlyCollection<IDirtableGridCell<T>> Cells { get; }
        public bool IsGlobalChange { get; }
        
        static public CellsDirtiedEventArgs<T> Change(IDirtableGridCell<T> cell) => new CellsDirtiedEventArgs<T>(new []{cell}, false);
        static public CellsDirtiedEventArgs<T> Change(IEnumerable<IDirtableGridCell<T>> cells) => new CellsDirtiedEventArgs<T>(cells.ToList().AsReadOnly(), false);
        static public CellsDirtiedEventArgs<T> GlobalChange() => new CellsDirtiedEventArgs<T>(Array.Empty<IDirtableGridCell<T>>(), true);
        
        public CellsDirtiedEventArgs(IReadOnlyCollection<IDirtableGridCell<T>> cells, bool isGlobalChange)
        {
            Cells = cells;
            IsGlobalChange = isGlobalChange;
        }
    }
}