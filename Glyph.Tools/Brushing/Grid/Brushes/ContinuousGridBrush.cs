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
        private readonly IndexRange _brushRange;

        private Point _previousGridPoint;
        private Vector2 _previousWorldPoint;

        public ContinuousGridBrush(Point? size = null)
        {
            (int x, int y) = size ?? new Point(1, 1);
            _brushRange = new IndexRange(new []{-y / 2, -x / 2}, new[] { y, x });
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
            GridIntersection segmentGridIntersection = canvas.Intersection(segment);

            IIndexEnumerator intersectionEnumerator = segmentGridIntersection.GetIndexEnumerator();
            int[] indexes = intersectionEnumerator.GetResetIndex();
            while (intersectionEnumerator.MoveIndex(indexes))
            {
                int y = indexes[0];
                int x = indexes[1];

                Quad cellShape = canvas.ToWorldRange(x, y, 1, 1);
                if (!cellShape.Intersects(segment))
                    continue;

                for (int i = 0; i < _brushRange.Lengths[0]; i++)
                    for (int j = 0; j < _brushRange.Lengths[1]; j++)
                    {
                        Point gridPoint = new Point(x + _brushRange.StartingIndexes[1] + j, y + _brushRange.StartingIndexes[0] + i);
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