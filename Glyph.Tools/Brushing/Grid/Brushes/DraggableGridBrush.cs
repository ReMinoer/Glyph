using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class DraggableGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        public override bool CanEndApply(IWriteableArray<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            return base.CanEndApply(canvas, args, paint) && paint.CanApply(canvas, args);
        }

        public override void EndApply(IWriteableArray<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            paint.Apply(canvas, args);
        }
    }
}