using Glyph.Space;
using Glyph.Tools.Brushing.Base;

namespace Glyph.Tools.Brushing.Grid.Brushes.Base
{
    public abstract class GridBrushBase<TCell, TPaint> : BrushBase<IWriteableGrid<TCell>, IGridBrushArgs, TPaint>, IGridBrush<IWriteableGrid<TCell>, TCell, TPaint>
        where TPaint : IPaint
    {
    }
}