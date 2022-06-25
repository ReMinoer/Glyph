using System;

namespace Glyph.Tools.UndoRedo
{
    public class NullUndoRedoStack : IUndoRedoStack
    {
        public void Execute(IUndoRedo undoRedo) {}
        public void Push(IUndoRedo undoRedo) {}
    }
}