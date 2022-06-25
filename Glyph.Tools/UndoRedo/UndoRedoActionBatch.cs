using System;
using System.Collections.Generic;

namespace Glyph.Tools.UndoRedo
{
    public class UndoRedoActionBatch : IUndoRedoActionBatch
    {
        public string Description { get; }
        private readonly List<Action> _redoList = new List<Action>();
        private readonly List<Action> _undoList = new List<Action>();

        public UndoRedoActionBatch(string description)
        {
            Description = description;
        }

        public void Push(Action redo, Action undo)
        {
            _redoList.Add(redo);
            _undoList.Insert(0, undo);
        }

        public void Execute(Action redo, Action undo)
        {
            redo?.Invoke();
            Push(redo, undo);
        }

        public void Undo()
        {
            foreach (Action undo in _undoList)
                undo();
        }

        public void Redo()
        {
            foreach (Action redo in _redoList)
                redo();
        }

        public override string ToString() => Description;
    }
}