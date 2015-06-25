using System;

namespace Glyph
{
    public interface IHandleInput
    {
        bool Enabled { get; }
        event EventHandler EnabledChanged;
        void HandleInput();
    }
}