using Glyph.Messaging;

namespace Glyph.Scripting
{
    public class OnEnterTrigger : Message
    {
        public ITriggerArea TriggerArea { get; private set; }
        public IActor Actor { get; private set; }

        public OnEnterTrigger(ITriggerArea triggerArea, IActor actor)
        {
            TriggerArea = triggerArea;
            Actor = actor;
        }
    }
}