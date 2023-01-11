using System;

namespace Glyph.Tools.UndoRedo
{
    static public class UndoRedoStackExtension
    {
        static public void Push(this IUndoRedoStack undoRedoStack, string description, Action redoAction, Action undoAction, Action doDispose = null, Action undoDispose = null)
        {
            undoRedoStack?.Push(new UndoRedoAction(description, redoAction, undoAction, doDispose, undoDispose));
        }

        static public void Execute(this IUndoRedoStack undoRedoStack, string description, Action doAction, Action undoAction, Action doDispose = null, Action undoDispose = null)
        {
            doAction();
            undoRedoStack.Push(description, doAction, undoAction, doDispose, undoDispose);
        }
    }
}