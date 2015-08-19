using System;
using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public interface ISchedulerProfile : IEnumerable<Predicate<object>>
    {
    }
}