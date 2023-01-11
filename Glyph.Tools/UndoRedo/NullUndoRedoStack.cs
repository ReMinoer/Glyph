namespace Glyph.Tools.UndoRedo
{
    public class NullUndoRedoStack : IUndoRedoStack
    {
        public void Push(IUndoRedo undoRedo) {}
    }
}