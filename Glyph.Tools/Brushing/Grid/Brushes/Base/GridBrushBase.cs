using Glyph.Space;
using Glyph.Tools.Brushing.Base;
using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid.Brushes.Base
{
    public abstract class GridBrushBase<TCell, TPaint> : BrushBase<IWriteableGrid<TCell>, IGridBrushArgs, TPaint>, IGridBrush<TCell, TPaint>
        where TPaint : IPaint
    {
        public override bool CanStartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => canvas.ContainsIndexes(args.GridPoint.Y, args.GridPoint.X);
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => canvas.ContainsIndexes(args.GridPoint.Y, args.GridPoint.X);
    }
}