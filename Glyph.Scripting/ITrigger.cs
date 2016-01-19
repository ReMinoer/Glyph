using System;

namespace Glyph.Scripting
{
    public interface ITrigger
    {
        string Name { get; set; }
        bool Active { get; }
        bool SingleUse { get; }
        event Action<ITrigger> Triggered;
    }
}