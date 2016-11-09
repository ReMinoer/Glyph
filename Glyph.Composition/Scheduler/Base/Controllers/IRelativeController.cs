namespace Glyph.Composition.Scheduler.Base.Controllers
{
    public interface IRelativeController<in T>
    {
        void Before(T item);
        void After(T item);
    }

    public interface IRelativeController<out TController, in T> : IRelativeController<T>
    {
        new TController Before(T item);
        new TController After(T item);
    }
}