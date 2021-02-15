using System;

namespace Glyph.Core.Scheduler
{
    public interface IGlyphSchedulerController<in T, out TController>
    {
        TController Before(T item, float? weight = null);
        TController After(T item, float? weight = null);
        TController Before<TItems>(float? weight = null);
        TController After<TItems>(float? weight = null);
        TController Before(Type type, float? weight = null);
        TController After(Type type, float? weight = null);
    }
}