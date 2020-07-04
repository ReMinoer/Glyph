using System;

namespace Glyph
{
    public interface INotifyDisposed
    {
        bool IsDisposed { get; }
        event EventHandler Disposed;
    }
}