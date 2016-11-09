using System;
using Glyph.Composition.Scheduler.Base.Controllers;

namespace Glyph.Composition.Scheduler
{
    public interface IGlyphSchedulerController<out TController, in TInterface, in TDelegate> : IRelativeController<TController, TDelegate>, IPriorityController<TController>
    {
        TController Before(TInterface item);
        TController After(TInterface item);
        TController Before<T>() where T : TInterface;
        TController After<T>() where T : TInterface;
        TController Before(Type type);
        TController After(Type type);
    }
}