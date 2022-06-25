namespace Glyph.Tools.UndoRedo
{
    public interface IUndoRedoStack
    {
        void Execute(IUndoRedo undoRedo);
        void Push(IUndoRedo undoRedo);
    }
}