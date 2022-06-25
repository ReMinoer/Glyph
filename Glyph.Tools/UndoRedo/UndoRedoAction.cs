using System;

namespace Glyph.Tools.UndoRedo
{
    public class UndoRedoAction : IUndoRedo
    {
        public string Description { get; }
        private readonly Action _redo;
        private readonly Action _undo;

        public UndoRedoAction(string description, Action redo, Action undo)
        {
            Description = description;
            _redo = redo;
            _undo = undo;
        }

        public void Undo() { _undo?.Invoke(); }
        public void Redo() { _redo?.Invoke(); }

        public override string ToString() => Description;
    }
}