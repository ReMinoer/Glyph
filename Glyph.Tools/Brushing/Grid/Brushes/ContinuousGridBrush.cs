using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class ContinuousGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private Point _currentPoint;

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            TryApply(canvas, args, paint);
        }

        public override void UpdateApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            if (_currentPoint != args.Point)
                TryApply(canvas, args, paint);
        }

        private void TryApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            if (!paint.CanApply(canvas, args))
                return;

            paint.Apply(canvas, args);
            _currentPoint = args.Point;
        }
        
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => true;
    }
}