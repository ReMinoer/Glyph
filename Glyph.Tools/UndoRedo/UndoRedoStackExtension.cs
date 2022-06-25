using System;

namespace Glyph.Tools.UndoRedo
{
    static public class UndoRedoStackExtension
    {
        static public void Execute(this IUndoRedoStack undoRedoStack, string description, Action doAction, Action undoAction)
        {
            if (undoRedoStack is null)
                doAction();
            else
                undoRedoStack.Execute(new UndoRedoAction(description, doAction, undoAction));
        }

        static public void Push(this IUndoRedoStack undoRedoStack, string description, Action redoAction, Action undoAction)
        {
            undoRedoStack?.Push(new UndoRedoAction(description, redoAction, undoAction));
        }
    }
}