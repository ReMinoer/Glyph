using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.UI.Controls;

namespace Glyph.UI.Menus
{
    public interface IMenu : IGlyphComponent, IEnumerable<IButton>
    {
        int SelectedIndex { get; }
        IButton SelectedControl { get; }
        int DefaultSelection { get; set; }
        event EventHandler<SelectionEventArgs> SelectionChanged;
        event EventHandler<SelectionEventArgs> SelectionTriggered;
        void Add(IButton component);
        void Remove(IButton component);
    }
}