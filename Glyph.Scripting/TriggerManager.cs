using System.Collections.Generic;

namespace Glyph.Scripting
{
    public class TriggerManager : Dictionary<string, Trigger>
    {
        private readonly ScriptManager _scriptManager;

        public TriggerManager(ScriptManager scriptManager)
        {
            _scriptManager = scriptManager;
        }

        public void Add(string name, bool uniqueUse)
        {
            var trigger = new Trigger(name, uniqueUse);
            trigger.Enabled += _scriptManager.TriggerOnEnabled;
            Add(name, new Trigger(name, uniqueUse));
        }

        public void Add(string name, int x, int y, int w, int h, int layer, bool uniqueUse)
        {
            var triggerZone = new TriggerZone(name, x, y, w, h, layer, uniqueUse);
            triggerZone.Enabled += _scriptManager.TriggerOnEnabled;
            Add(name, triggerZone);
        }
    }
}