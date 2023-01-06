using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Glyph.Tools.UndoRedo;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class DraggableGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            return paint.CanApply(canvas, args);
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint, IUndoRedoStack undoRedoStack)
        {
            object undoData = paint.GetUndoData(canvas, args);

            undoRedoStack?.Push($"Apply paint {paint} with brush {this} on canvas {canvas}.",
                () => paint.Apply(canvas, args),
                () => paint.Undo(canvas, args, undoData));
        }
    }
}