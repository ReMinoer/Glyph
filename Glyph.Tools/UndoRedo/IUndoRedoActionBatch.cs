using System;

namespace Glyph.Tools.UndoRedo
{
    public interface IUndoRedoActionBatch : IUndoRedo
    {
        void Push(Action redo, Action undo);
        void Execute(Action redo, Action undo);
    }
}