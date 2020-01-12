using Glyph.Space;

namespace Glyph.Tools.Brushing.Grid
{
    public interface IGridBrush<TCell, in TPaint> : IBrush<IWriteableGrid<TCell>, IGridBrushArgs, TPaint>
        where TPaint : IPaint
    {
    }
}