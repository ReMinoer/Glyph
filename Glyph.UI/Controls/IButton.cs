using System;
using Glyph.Composition;

namespace Glyph.UI.Controls
{
    public interface IButton : IControl, ILoadContent
    {
        Text Text { get; }
        IFrame Frame { get; }
        bool Pressed { get; }
        bool Hover { get; set; }
        event EventHandler Triggered;
        event EventHandler Released;
        event EventHandler Entered;
        event EventHandler Leaved;
    }
}