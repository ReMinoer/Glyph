using System.Collections.Generic;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Glyph.Tools.UndoRedo;
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
        private UndoRedoBatch _undoRedoBatch;

        public ContinuousGridBrush(Point? size = null)
        {
            (int x, int y) = size ?? new Point(1, 1);
            _brushRange = new IndexRange(new []{-y / 2, -x / 2}, new[] { y, x });
        }

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            _previousGridPoint = args.GridPoint;
            _previousWorldPoint = args.WorldPoint;
            _undoRedoBatch = new UndoRedoBatch($"Apply paint {paint} with brush {this} on canvas {canvas}.");

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
            var segment = new Segment(args.WorldPoint, _previousWorldPoint);

            IEnumerable<int[]> indexIntersection = canvas.IndexIntersection(segment);
            foreach (int[] indexes in indexIntersection)
            {
                int x = indexes[1];
                int y = indexes[0];

                for (int i = 0; i < _brushRange.Lengths[0]; i++)
                    for (int j = 0; j < _brushRange.Lengths[1]; j++)
                    {
                        var gridPoint = new Point(x + _brushRange.StartingIndex[1] + j, y + _brushRange.StartingIndex[0] + i);
                        Vector2 worldPoint = canvas.ToWorldPoint(gridPoint);

                        var cellArgs = new GridBrushArgs(gridPoint, worldPoint);
                        if (!paint.CanApply(canvas, cellArgs))
                            continue;

                        object undoData = paint.GetUndoData(canvas, cellArgs);

                        _undoRedoBatch.Execute($"Apply paint {paint} with brush {this} on canvas {canvas} at position {gridPoint}.",
                            () => paint.Apply(canvas, cellArgs),
                            () => paint.Undo(canvas, cellArgs, undoData));
                    }
            }
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint, IUndoRedoStack undoRedoStack)
        {
            undoRedoStack?.Push(_undoRedoBatch);
        }

        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => true;
    }
}