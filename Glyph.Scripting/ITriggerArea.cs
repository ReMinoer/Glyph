using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Math;

namespace Glyph.Scripting
{
    public interface ITriggerArea : ITrigger, IShape
    {
        bool Enabled { get; set; }
        void UpdateStatus(IEnumerable<IActor> actors);
    }

    public interface ITriggerArea<out T> : ITriggerArea, IShapedObject<T>
        where T : IShape
    {
    }
}