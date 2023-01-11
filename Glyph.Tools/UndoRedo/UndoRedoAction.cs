using System;

namespace Glyph.Tools.UndoRedo
{
    public class UndoRedoAction : IUndoRedo
    {
        public string Description { get; }
        private readonly Action _redo;
        private readonly Action _undo;
        private readonly Action _redoDispose;
        private readonly Action _undoDispose;
        private bool _isDone;

        public UndoRedoAction(string description, Action redo, Action undo, Action redoDispose, Action undoDispose, bool alreadyDone = true)
        {
            Description = description;
            _redo = redo;
            _undo = undo;
            _redoDispose = redoDispose;
            _undoDispose = undoDispose;
            _isDone = alreadyDone;
        }

        public override string ToString() => Description;

        public void Undo()
        {
            if (!_isDone)
                throw new InvalidOperationException($"Action \"{Description}\" is already undone.");

            _undo?.Invoke();
            _isDone = false;
        }

        public void Redo()
        {
            if (_isDone)
                throw new InvalidOperationException($"Action \"{Description}\" is not undone and cannot be redone.");

            _redo?.Invoke();
            _isDone = true;
        }

        public void Dispose()
        {
            if (_isDone)
                _redoDispose?.Invoke();
            else
                _undoDispose?.Invoke();
        }
    }
}