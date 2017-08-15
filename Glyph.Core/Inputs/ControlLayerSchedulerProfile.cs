using System;
using Diese.Scheduling;
using Fingear;

namespace Glyph.Core.Inputs
{
    public class ControlLayerSchedulerProfile
    {
        static public IReadOnlyScheduler<Predicate<object>> Get()
        {
            var scheduler = new Scheduler<Predicate<object>>();
            scheduler.Plan(x => x is ControlLayer controlLayer && controlLayer.Tags.Contains(ControlLayerTag.Tools));
            scheduler.Plan(x => x is ControlLayer controlLayer && controlLayer.Tags.Contains(ControlLayerTag.Debug));
            scheduler.Plan(x => x is ControlLayer controlLayer && controlLayer.Tags.Contains(ControlLayerTag.Ui));
            return new ReadOnlyScheduler<Predicate<object>>(scheduler);
        }
    }
}