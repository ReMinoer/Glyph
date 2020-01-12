using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class ContinuousGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private Point _previousGridPoint;
        private Vector2 _previousWorldPoint;

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            _previousGridPoint = args.GridPoint;
            _previousWorldPoint = args.WorldPoint;

            TryApply(canvas, args, paint);
        }

        public override void UpdateApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            if (_previousGridPoint != args.GridPoint)
                TryApply(canvas, args, paint);

            _previousGridPoint = args.GridPoint;
            _previousWorldPoint = args.WorldPoint;
        }

        private void TryApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            // TODO: Use better grid cell-segment intersection algorithm
            var segment = new Segment(args.WorldPoint, _previousWorldPoint);
            TopLeftRectangle segmentBox = MathUtils.GetBoundingBox(args.WorldPoint, _previousWorldPoint);
            Rectangle gridRectangle = canvas.ToGridRange(segmentBox);

            for (int i = 0; i < gridRectangle.Height; i++)
                for (int j = 0; j < gridRectangle.Width; j++)
                {
                    TopLeftRectangle cellRectangle = canvas.ToWorldRange(j + gridRectangle.X, i + gridRectangle.Y, 1, 1);
                    if (!cellRectangle.Intersects(segment))
                        continue;

                    if (paint.CanApply(canvas, args))
                        paint.Apply(canvas, args);
                }
        }
        
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => true;
    }
}