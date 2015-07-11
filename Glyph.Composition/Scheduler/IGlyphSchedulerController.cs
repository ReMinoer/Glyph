using System;
using Glyph.Composition.Scheduler.Base;

namespace Glyph.Composition.Scheduler
{
    public interface IGlyphSchedulerController<in TInterface, in TDelegate> : ISchedulerController<TDelegate>
    {
        void Before(TInterface item);
        void After(TInterface item);
        void Before<T>() where T : TInterface;
        void After<T>() where T : TInterface;
        void Before(Type type);
        void After(Type type);
    }
}