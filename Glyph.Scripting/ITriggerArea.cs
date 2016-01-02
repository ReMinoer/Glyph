using System.Collections.Generic;
using Glyph.Math;

namespace Glyph.Scripting
{
    public delegate void TriggerAreaEventHandler(ITrigger trigger, IActor actor);

    public interface ITriggerArea : ITrigger, IShape
    {
        bool Enabled { get; set; }
        bool Visible { get; set; }
        new event TriggerAreaEventHandler Triggered;
        void UpdateStatus(IEnumerable<IActor> actors);
    }
}