using System;

namespace Glyph.UI.Menus
{
    public class SelectionEventArgs : EventArgs
    {
        public int Selection { get; set; }

        public SelectionEventArgs(int selection)
        {
            Selection = selection;
        }
    }
}