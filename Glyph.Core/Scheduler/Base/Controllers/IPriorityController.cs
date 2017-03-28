namespace Glyph.Core.Scheduler.Base.Controllers
{
    public interface IPriorityController
    {
        void AtStart();
        void AtEnd();
    }

    public interface IPriorityController<out TController> : IPriorityController
    {
        new TController AtStart();
        new TController AtEnd();
    }
}