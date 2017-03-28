using System;
using System.Collections.Generic;
using Glyph.Composition;

namespace Glyph.Core.Scheduler.Base
{
    public class SchedulerProfile<T> : List<Predicate<object>>, ISchedulerProfile
        where T : IGlyphComponent
    {
        protected void Add<TComponent>()
            where TComponent : T
        {
            Add(x => x is TComponent);
        }
    }
}