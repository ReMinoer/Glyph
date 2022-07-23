using System;

namespace Glyph.Tools.UndoRedo
{
    public interface IUndoRedo : IDisposable
    {
        string Description { get; }
        void Undo();
        void Redo();
    }
}