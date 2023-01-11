namespace Glyph.Tools.UndoRedo
{
    public interface IUndoRedoStack
    {
        void Push(IUndoRedo undoRedo);
    }
}