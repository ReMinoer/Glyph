using System;
using System.Collections.Generic;

namespace Glyph.Scheduling
{
    public interface IGlyphSchedulerController<in T, out TController>
    {
        TController Before(T task, float? weight = null);
        TController After(T task, float? weight = null);
        TController Before(IEnumerable<T> tasks, float? weight = null);
        TController After(IEnumerable<T> tasks, float? weight = null);
        TController Before<TItems>(float? weight = null);
        TController After<TItems>(float? weight = null);
        TController Before(Type type, float? weight = null);
        TController After(Type type, float? weight = null);
    }
}