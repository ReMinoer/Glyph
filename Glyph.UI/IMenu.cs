using System;
using Glyph.Composition;
using Glyph.UI.Controls;
using Glyph.UI.Menus;

namespace Glyph.UI
{
    public interface IMenu : IGlyphComposite<IButton>
    {
        int SelectedIndex { get; }
        IButton SelectedControl { get; }
        int DefaultSelection { get; set; }
        event EventHandler<SelectionEventArgs> SelectionChanged;
        event EventHandler<SelectionEventArgs> SelectionTriggered;
    }
}