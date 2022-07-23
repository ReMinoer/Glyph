using System;

namespace Glyph.Tools.UndoRedo
{
    static public class UndoRedoStackExtension
    {
        static public void Execute(this IUndoRedoStack undoRedoStack, string description, Action doAction, Action undoAction, Action doDispose = null, Action undoDispose = null)
        {
            if (undoRedoStack is null)
                doAction();
            else
                undoRedoStack.Execute(new UndoRedoAction(description, doAction, undoAction, doDispose, undoDispose, alreadyDone: false));
        }

        static public void Push(this IUndoRedoStack undoRedoStack, string description, Action redoAction, Action undoAction, Action doDispose = null, Action undoDispose = null)
        {
            undoRedoStack?.Push(new UndoRedoAction(description, redoAction, undoAction, doDispose, undoDispose, alreadyDone: true));
        }
    }
}