using Diese;
using Glyph.Composition;

namespace Glyph.Core.Scheduler
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