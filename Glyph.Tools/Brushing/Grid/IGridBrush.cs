using Glyph.Space;

namespace Glyph.Tools.Brushing.Grid
{
    public interface IGridBrush<in TCanvas, TCell, in TPaint> : IBrush<TCanvas, IGridBrushArgs, TPaint>
        where TCanvas : IWriteableGrid<TCell>
        where TPaint : IPaint
    {
    }
}