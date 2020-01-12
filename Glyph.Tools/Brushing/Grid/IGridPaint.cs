using Glyph.Space;

namespace Glyph.Tools.Brushing.Grid
{
    public interface IGridPaint<TCell> : IPaint<IWriteableGrid<TCell>, IGridBrushArgs>
    {
    }
}