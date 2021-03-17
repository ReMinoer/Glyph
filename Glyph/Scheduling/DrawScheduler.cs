using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Scheduling.Base;
using Taskete.Rules;
using Taskete.Schedulers;

namespace Glyph.Scheduling
{
    public delegate void DrawDelegate(IDrawer drawer);
    
    public interface IDrawTask
    {
        bool Rendered { get; }
        float RenderDepth { get; }
        ISceneNode SceneNode { get; }
        Predicate<IDrawer> DrawPredicate { get; set; }
        IFilter<IDrawClient> DrawClientFilter { get; set; }
        event EventHandler RenderDepthChanged;
        void Draw(IDrawer drawer);
    }

    public class DrawTask : DelegateTaskBase<DrawDelegate>, IDrawTask
    {
        public bool Rendered { get; set; } = true;
        public float? RenderDepthOverride { get; set; }
        public ISceneNode SceneNode { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public float RenderDepth => RenderDepthOverride ?? SceneNode?.Depth ?? 0f;
        
        public event EventHandler RenderDepthChanged;

        public DrawTask(DrawDelegate taskDelegate)
            : base(taskDelegate)
        {
        }

        public void Draw(IDrawer drawer) => TaskDelegate(drawer);
    }

    public class DrawScheduler : GlyphSchedulerBase<IDrawTask, DrawDelegate>
    {
        private readonly LinearScheduler<IDrawTask> _scheduler;

        public DrawScheduler()
            : base(new LinearScheduler<IDrawTask>(), x => new DrawTask(x))
        {
            _scheduler = (LinearScheduler<IDrawTask>)Scheduler;

            var sortRule = new SortRule<IDrawTask, float>(Scheduler.Tasks, x => x.RenderDepth, Comparer<float>.Default,
                (x, h) => x.RenderDepthChanged += h, (x, h) => x.RenderDepthChanged -= h)
            {
                Weight = 10f,
                MustBeApplied = true
            };
            Scheduler.Rules.Add(sortRule);
        }

        public IEnumerable<IDrawTask> GetSchedule(IDrawer drawer)
        {
            return _scheduler.Schedule.Where(x => x.Displayed(drawer, drawer.Client, x.SceneNode));
        }

        new public DelegateController Plan(DrawDelegate taskDelegate)
        {
            var task = (DrawTask)GetOrAddDelegateTask(taskDelegate);
            Plan(task);
            return new DelegateController(this, new []{ task });
        }

        public class DelegateController : ControllerBase<DelegateController, DrawTask>
        {
            public DelegateController(GlyphSchedulerBase<IDrawTask, DrawDelegate> glyphScheduler, IEnumerable<DrawTask> controlledTasks)
                : base(glyphScheduler, controlledTasks) { }

            protected override DelegateController ReturnedController => this;

            public DelegateController InScene(ISceneNode sceneNode)
            {
                foreach (DrawTask controlledTask in ControlledTasks)
                {
                    controlledTask.SceneNode = sceneNode;
                }

                return ReturnedController;
            }

            public DelegateController AtDepth(float depth)
            {
                foreach (DrawTask controlledTask in ControlledTasks)
                {
                    controlledTask.RenderDepthOverride = depth;
                }

                return ReturnedController;
            }
        }
    }
}