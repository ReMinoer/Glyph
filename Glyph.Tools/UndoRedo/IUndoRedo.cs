namespace Glyph.Tools.UndoRedo
{
    public interface IUndoRedo
    {
        string Description { get; }
        void Undo();
        void Redo();
    }
}