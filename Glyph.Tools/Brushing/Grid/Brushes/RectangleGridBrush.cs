using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class RectangleGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private Point _startPoint;

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            _startPoint = args.GridPoint;
        }

        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            Rectangle rectangle = MathUtils.GetBoundingBox(args.GridPoint, _startPoint).ClampToRectangle(canvas.IndexesBounds());

            for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
                for (int j = rectangle.Left; j <= rectangle.Right; j++)
                {
                    var point = new Point(j, i);
                    if (!paint.CanApply(canvas, new GridBrushArgs(point, canvas.ToWorldPoint(point))))
                        return false;
                }

            return true;
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint, IUndoRedoStack undoRedoStack)
        {
            Rectangle rectangle = MathUtils.GetBoundingBox(args.GridPoint, _startPoint).ClampToRectangle(canvas.IndexesBounds());
            var undoRedoActionBatch = new UndoRedoActionBatch($"Apply paint {paint} with brush {this} on canvas {canvas}.");

            for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
                for (int j = rectangle.Left; j <= rectangle.Right; j++)
                {
                    var gridPoint = new Point(j, i);
                    Vector2 worldPoint = canvas.ToWorldPoint(gridPoint);

                    paint.Apply(canvas, new GridBrushArgs(gridPoint, worldPoint), undoRedoActionBatch);
                }

            undoRedoStack?.Push(undoRedoActionBatch);
        }
    }
}