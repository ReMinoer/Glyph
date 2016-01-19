using System.Collections.Generic;
using Glyph.Math;

namespace Glyph.Scripting
{
    public interface ITriggerArea : ITrigger, IShape
    {
        bool Enabled { get; set; }
        bool Visible { get; set; }
        void UpdateStatus(IEnumerable<IActor> actors);
    }
}