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
            Rectangle gridRectangle = canvas.ToGridRange(segmentBox).ClampToRectangle(canvas.IndexesBounds());
            
            for (int i = gridRectangle.Top; i <= gridRectangle.Bottom; i++)
                for (int j = gridRectangle.Left; j <= gridRectangle.Right; j++)
                {
                    var gridPoint = new Point(j, i);

                    TopLeftRectangle cellRectangle = canvas.ToWorldRange(gridPoint.X, gridPoint.Y, 1, 1);
                    if (!cellRectangle.Intersects(segment))
                        continue;

                    Vector2 worldPoint = canvas.ToWorldPoint(gridPoint);

                    var cellArgs = new GridBrushArgs(gridPoint, worldPoint);
                    if (paint.CanApply(canvas, cellArgs))
                        paint.Apply(canvas, cellArgs);
                }
        }
        
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => true;
    }
}