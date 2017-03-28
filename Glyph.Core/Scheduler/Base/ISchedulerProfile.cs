using System;
using System.Collections.Generic;

namespace Glyph.Core.Scheduler.Base
{
    public interface ISchedulerProfile : IEnumerable<Predicate<object>>
    {
    }
}