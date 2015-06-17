using System;

namespace Glyph
{
    public interface IUpdate : IGlyphComponent
    {
        bool Enabled { get; }
        event EventHandler EnabledChanged;
        void Update();
    }
}