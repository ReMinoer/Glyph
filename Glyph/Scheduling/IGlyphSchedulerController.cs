using System;
using System.Collections.Generic;

namespace Glyph.Scheduling
{
    public interface IGlyphSchedulerController<in T, out TController>
    {
        TController Before(T task);
        TController After(T task);
        TController Before(IEnumerable<T> tasks);
        TController After(IEnumerable<T> tasks);
        TController Before<TItems>();
        TController After<TItems>();
        TController Before(Type type);
        TController After(Type type);
        TController WithWeight(float weight);
        TController Mandatory();
    }
}