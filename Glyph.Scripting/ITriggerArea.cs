using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;

namespace Glyph.Scripting
{
    public delegate void TriggerAreaEventHandler(ITrigger trigger, IActor actor);

    public interface ITriggerArea : ITrigger, IShape
    {
        bool Enabled { get; set; }
        bool Visible { get; set; }
        void UpdateStatus(IEnumerable<IActor> actors);
        new event TriggerAreaEventHandler Triggered;
    }
}