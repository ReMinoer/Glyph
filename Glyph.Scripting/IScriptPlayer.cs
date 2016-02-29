using System.Collections.Generic;
using Glyph.Composition;

namespace Glyph.Scripting
{
    public interface IScriptPlayer : IEnableable, IUpdate, ITimeUnscalable
    {
        Dictionary<object, IActor> Actors { get; }
        IReadOnlyDictionary<string, ITrigger> Triggers { get; }
        IReadOnlyDictionary<string, ITriggerArea> TriggerAreas { get; }
        void RegisterTrigger(ITrigger trigger);
        void RegisterArea(ITriggerArea triggerArea);
        void UnregisterTrigger(ITrigger trigger);
        void UnregisterTrigger(string name);
        void CleanTriggers();
    }
}