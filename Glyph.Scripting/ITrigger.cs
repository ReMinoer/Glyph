namespace Glyph.Scripting
{
    public delegate void TriggerEventHandler(ITrigger trigger);

    public interface ITrigger
    {
        string Name { get; set; }
        bool Active { get; }
        bool SingleUse { get; }
        event TriggerEventHandler Triggered;
    }
}