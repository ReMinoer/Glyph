using System.Collections.Generic;
using System.Linq;

namespace Glyph.Tools.UndoRedo
{
    public class UndoRedoBatch : IUndoRedoBatch
    {
        private readonly List<IUndoRedo> _undoRedoList = new List<IUndoRedo>();
        public string Description { get; }

        public UndoRedoBatch(string description)
        {
            Description = description;
        }

        public override string ToString() => Description;

        public void Push(IUndoRedo undoRedo)
        {
            _undoRedoList.Add(undoRedo);
        }

        void IUndoRedo.Undo()
        {
            foreach (IUndoRedo undoRedo in Enumerable.Reverse(_undoRedoList))
                undoRedo.Undo();
        }

        void IUndoRedo.Redo()
        {
            foreach (IUndoRedo undoRedo in _undoRedoList)
                undoRedo.Redo();
        }

        public void Dispose()
        {
            foreach (IUndoRedo undoRedo in Enumerable.Reverse(_undoRedoList))
                undoRedo.Dispose();
        }
    }
}