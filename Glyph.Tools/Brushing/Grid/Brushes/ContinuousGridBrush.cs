using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class ContinuousGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private readonly ArrayRange _brushRange;

        private Point _previousGridPoint;
        private Vector2 _previousWorldPoint;

        public ContinuousGridBrush(Point? size = null)
        {
            (int x, int y) = size ?? new Point(1, 1);
            _brushRange = new ArrayRange(new []{-y / 2, -x / 2}, new[] { y, x });
        }

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

                    for (int y = 0; y < _brushRange.Lengths[0]; y++)
                        for (int x = 0; x < _brushRange.Lengths[1]; x++)
                        {
                            gridPoint = new Point(j + _brushRange.StartingIndexes[1] + x, i + _brushRange.StartingIndexes[0] + y);
                            Vector2 worldPoint = canvas.ToWorldPoint(gridPoint);

                            var cellArgs = new GridBrushArgs(gridPoint, worldPoint);
                            if (paint.CanApply(canvas, cellArgs))
                                paint.Apply(canvas, cellArgs);
                        }

                }
        }
        
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => true;
    }
}