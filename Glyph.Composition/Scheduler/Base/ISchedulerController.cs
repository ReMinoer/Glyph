namespace Glyph.Composition.Scheduler.Base
{
    public interface ISchedulerController<in T>
    {
        void AtStart();
        void AtEnd();
        void Before(T item);
        void After(T item);
    }
}