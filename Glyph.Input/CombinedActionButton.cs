using System.Collections.Generic;
using System.Linq;

namespace Glyph.Input
{
    public class CombinedActionButton : IActionButton
    {
        public List<IActionButton> ActionButtons { get; set; }

        public CombinedActionButton(string name)
        {
            Name = name;
            ActionButtons = new List<IActionButton>();
        }

        public string Name { get; set; }

        public bool IsPressed(InputManager input)
        {
            if (!ActionButtons.Any())
                return false;

            foreach (IActionButton a in ActionButtons)
                if (!a.IsPressed(input))
                    return false;
            return true;
        }

        public bool IsDownNow(InputManager input)
        {
            if (!ActionButtons.Any())
                return false;

            foreach (IActionButton a in ActionButtons)
                if (!a.IsDownNow(input))
                    return false;
            return true;
        }
    }
}