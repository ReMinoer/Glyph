using System.Collections.Generic;

namespace Glyph.Scripting
{
    public class TriggerManager : Dictionary<string, Trigger>
    {
        public void Add(string name, bool uniqueUse)
        {
            Add(name, new Trigger(name, uniqueUse));
        }

        public void Add(string name, int x, int y, int w, int h, int layer, bool uniqueUse)
        {
            Add(name, new TriggerZone(name, x, y, w, h, layer, uniqueUse));
        }
    }
}