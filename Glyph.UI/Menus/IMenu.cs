using System;
using Glyph.Composition;
using Glyph.UI.Controls;

namespace Glyph.UI.Menus
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