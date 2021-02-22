using Glyph.Composition;
using Glyph.Core.Schedulers.Base;
using Glyph.Scheduling;
using Niddle;

namespace Glyph.Core.Schedulers
{
    public class ComponentSchedulerHandler
    {
        public InitializeComponentScheduler Initialize { get; }
        public LoadContentComponentScheduler LoadContent { get; }
        public UpdateComponentScheduler Update { get; }
        public DrawComponentScheduler Draw { get; }

        public ComponentSchedulerHandler(IDependencyResolver resolver)
        {
            Initialize = resolver.Resolve<InitializeComponentScheduler>();
            LoadContent = resolver.Resolve<LoadContentComponentScheduler>();
            Update = resolver.Resolve<UpdateComponentScheduler>();
            Draw = resolver.Resolve<DrawComponentScheduler>();
        }

        public void PlanComponent(IGlyphComponent component)
        {
            Initialize.Plan(component);
            if (component is ILoadContent loadContent)
                LoadContent.Plan(loadContent);
            if (component is IUpdate update)
                Update.Plan(update);
            if (component is IDraw draw)
                Draw.Plan(draw);
        }

        public void UnplanComponent(IGlyphComponent component)
        {
            Initialize.Unplan(component);
            if (component is ILoadContent loadContent)
                LoadContent.Unplan(loadContent);
            if (component is IUpdate update)
                Update.Unplan(update);
            if (component is IDraw draw)
                Draw.Unplan(draw);
        }
    }

    public class InitializeComponentScheduler : ComponentScheduler<IInitializeTask, InitializeDelegate>
    {
        public InitializeComponentScheduler(InitializeScheduler scheduler)
            : base(scheduler) { }
    }

    public class LoadContentComponentScheduler : AsyncComponentScheduler<ILoadContentTask, LoadContentDelegate, IContentLibrary>
    {
        public LoadContentComponentScheduler(LoadContentScheduler scheduler)
            : base(scheduler) { }
    }

    public class UpdateComponentScheduler : ComponentScheduler<IUpdateTask, UpdateDelegate>
    {
        public UpdateComponentScheduler(UpdateScheduler scheduler)
            : base(scheduler) { }
    }

    public class DrawComponentScheduler : ComponentScheduler<IDrawTask, DrawDelegate>
    {
        public DrawComponentScheduler(DrawScheduler scheduler)
            : base(scheduler) { }
    }
}