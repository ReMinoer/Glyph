using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

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
            object[,] undoDataArray = new object[rectangle.Bottom - rectangle.Top + 1, rectangle.Right - rectangle.Left + 1];
            
            undoDataArray.GetResetIndex(out int i, out int j);
            while (undoDataArray.MoveIndex(ref i, ref j))
                undoDataArray[i, j] = paint.GetUndoData(canvas, GetArgs(canvas, rectangle, i, j));
            
            undoRedoStack?.Execute($"Apply paint {paint} with brush {this} on canvas {canvas}.",
                () =>
                {
                    undoDataArray.GetResetIndex(out i, out j);
                    while (undoDataArray.MoveIndex(ref i, ref j))
                        paint.Apply(canvas, GetArgs(canvas, rectangle, i, j));
                },
                () =>
                {
                    undoDataArray.GetResetIndex(out i, out j);
                    while (undoDataArray.MoveIndex(ref i, ref j))
                        paint.Undo(canvas, GetArgs(canvas, rectangle, i, j), undoDataArray[i, j]);
                });
        }

        static private GridBrushArgs GetArgs(IGrid canvas, Rectangle rectangle, int i, int j)
        {
            var gridPoint = new Point(j + rectangle.Left, i + rectangle.Top);
            return new GridBrushArgs(gridPoint, canvas.ToWorldPoint(gridPoint));
        }
    }
}