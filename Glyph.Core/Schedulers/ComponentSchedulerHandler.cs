using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Core.Schedulers.Base;
using Glyph.Scheduling;
using Glyph.Scheduling.Base;
using Niddle;
using Stave;

namespace Glyph.Core.Schedulers
{
    public class ComponentSchedulerHandler
    {
        public InitializeComponentScheduler Initialize { get; }
        public LoadContentComponentScheduler LoadContent { get; }
        public UpdateComponentScheduler Update { get; }
        public DrawComponentScheduler Draw { get; }

        public ComponentSchedulerHandler(IDependencyResolver resolver, IDrawNode drawNode)
        {
            Initialize = resolver.Resolve<InitializeComponentScheduler>();
            LoadContent = resolver.Resolve<LoadContentComponentScheduler>();
            Update = resolver.Resolve<UpdateComponentScheduler>();
            Draw = resolver.WithInstance(drawNode).Resolve<DrawComponentScheduler>();
        }

        public void PlanComponent(IGlyphComponent component)
        {
            Initialize.Plan(component);
            if (component is ILoadContent loadContent)
                LoadContent.Plan(loadContent);
            if (component is IUpdate update)
                Update.Plan(update);

            // BUG: Inaccurate
            foreach (IDraw draw in component.AndAllChildren().OfType<IDraw>())
                Draw.Plan(draw);
        }

        public void UnplanComponent(IGlyphComponent component)
        {
            Initialize.Unplan(component);
            if (component is ILoadContent loadContent)
                LoadContent.Unplan(loadContent);
            if (component is IUpdate update)
                Update.Unplan(update);

            // BUG: Inaccurate
            foreach (IDraw draw in component.AndAllChildren().OfType<IDraw>())
                Draw.Unplan(draw);
        }
    }

    public class InitializeComponentScheduler : ComponentScheduler<IInitializeTask, InitializeDelegate>
    {
        public InitializeComponentScheduler(InitializeScheduler scheduler)
            : base(scheduler) { }
    }

    public class LoadContentComponentScheduler : AsyncComponentScheduler<ILoadContentTask, LoadContentAsyncDelegate, LoadContentDelegate, IContentLibrary>
    {
        public LoadContentComponentScheduler(LoadContentScheduler scheduler)
            : base(scheduler) { }
    }

    public class UpdateComponentScheduler : ComponentScheduler<IUpdateTask, UpdateDelegate>
    {
        public UpdateComponentScheduler(UpdateScheduler scheduler)
            : base(scheduler) { }
    }

    public class DrawComponentScheduler : IGlyphDelegateScheduler<IDrawTask, DrawDelegate, DrawScheduler.Controller, DrawScheduler.Controller>
    {
        private readonly DrawScheduler _scheduler;
        private readonly IGlyphDelegateScheduler<IDrawTask, DrawDelegate, DrawScheduler.Controller, DrawScheduler.Controller> _schedulerInterface;
        private readonly IDrawNode _parentDrawNode;

        public DrawComponentScheduler(DrawScheduler scheduler, IDrawNode parentDrawNode)
        {
            _scheduler = scheduler;
            _schedulerInterface = _scheduler;
            _parentDrawNode = parentDrawNode;
        }
        
        public GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller Plan(IDrawTask task) => _scheduler.Plan(task);
        public GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller Plan(IEnumerable<IDrawTask> tasks) => _scheduler.Plan(tasks);
        public DrawScheduler.DelegateController Plan(DrawDelegate taskDelegate) => _scheduler.Plan(taskDelegate).WithParentDrawNode(_parentDrawNode);
        public GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller Plan(Type taskType) => _scheduler.Plan(taskType);
        public GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller Plan<TTask>() => _scheduler.Plan<TTask>();
        
        void IGlyphScheduler<IDrawTask>.Plan(IDrawTask task) => Plan(task);
        void IGlyphDelegateScheduler<IDrawTask, DrawDelegate>.Plan(DrawDelegate taskDelegate) => Plan(taskDelegate);
        GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller IGlyphDelegateScheduler<IDrawTask, DrawDelegate, GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller, GlyphSchedulerBase<IDrawTask, DrawDelegate>.Controller>.Plan(DrawDelegate taskDelegate)
            => _schedulerInterface.Plan(taskDelegate);
        
        public void Unplan(IDrawTask task) => _scheduler.Unplan(task);
        public void Unplan(DrawDelegate taskDelegate) => _scheduler.Unplan(taskDelegate);
    }
}