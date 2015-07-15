using System;
using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public class SchedulerProfile<T> : List<Func<object, bool>>, ISchedulerProfile
        where T : IGlyphComponent
    {
        protected void Add<TComponent>()
            where TComponent : T
        {
            Add(x => x is TComponent);
        }
    }
}