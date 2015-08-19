namespace Glyph.Composition.Scheduler
{
    public interface IGlyphSchedulerAssigner : IBatchable
    {
        void AssignComponent(IGlyphComponent component);
        void AssignComponent(GlyphObject glyphObject);
        void RemoveComponent(IGlyphComponent component);
        void RemoveComponent(GlyphObject glyphObject);
        void ClearComponents();
    }
}