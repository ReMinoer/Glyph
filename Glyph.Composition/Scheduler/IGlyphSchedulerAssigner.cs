using Diese;
using Glyph.Composition.Scheduler.Base;

namespace Glyph.Composition.Scheduler
{
    public interface IGlyphSchedulerAssigner : IBatchTree
    {
        void AssignComponent(IGlyphComponent component);
        void AssignComponent(GlyphObject glyphObject);
        void RemoveComponent(IGlyphComponent component);
        void RemoveComponent(GlyphObject glyphObject);
        void ClearComponents();
    }
}