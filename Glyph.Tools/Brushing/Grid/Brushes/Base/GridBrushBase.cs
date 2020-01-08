using Glyph.Tools.Brushing.Base;
using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid.Brushes.Base
{
    public abstract class GridBrushBase<TCell, TPaint> : BrushBase<IWriteableArray<TCell>, IGridBrushArgs, TPaint>, IGridBrush<TCell, TPaint>
        where TPaint : IPaint
    {
        public override bool CanStartApply(IWriteableArray<TCell> canvas, IGridBrushArgs args, TPaint paint) => canvas.ContainsIndexes(args.Point.Y, args.Point.X);
        public override bool CanEndApply(IWriteableArray<TCell> canvas, IGridBrushArgs args, TPaint paint) => canvas.ContainsIndexes(args.Point.Y, args.Point.X);
    }
}