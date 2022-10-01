using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Diese.Collections;
using Glyph.Scheduling.Base;
using Taskete;
using Taskete.Rules;
using Taskete.Rules.Base;
using Taskete.Schedulers;

namespace Glyph.Scheduling
{
    public delegate void DrawDelegate(IDrawer drawer);

    public interface IDrawNode
    {
        IDrawNode ParentDrawNode { get; }
        int IndexOfDrawNode(IDrawNode task);
        event EventHandler DrawNodeHierarchyChanged;
    }
    
    public interface IDrawTask : IDrawNode
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
        private IDrawNode _parentDrawNode;
        public bool Rendered { get; set; } = true;
        public float? RenderDepthOverride { get; set; }
        public ISceneNode SceneNode { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public IDrawNode ParentDrawNode
        {
            get => _parentDrawNode;
            set
            {
                if (_parentDrawNode != null)
                    _parentDrawNode.DrawNodeHierarchyChanged -= OnParentNodeHierarchyChanged;

                _parentDrawNode = value;
                DrawNodeHierarchyChanged?.Invoke(this, EventArgs.Empty);

                if (_parentDrawNode != null)
                    _parentDrawNode.DrawNodeHierarchyChanged += OnParentNodeHierarchyChanged;
            }
        }

        private void OnParentNodeHierarchyChanged(object sender, EventArgs e) => DrawNodeHierarchyChanged?.Invoke(this, EventArgs.Empty);

        public float RenderDepth => RenderDepthOverride ?? SceneNode?.Depth ?? 0f;
        
        public event EventHandler DrawNodeHierarchyChanged;
        public event EventHandler RenderDepthChanged;

        public DrawTask(DrawDelegate taskDelegate)
            : base(taskDelegate)
        {
        }

        public void Draw(IDrawer drawer) => TaskDelegate(drawer);
        int IDrawNode.IndexOfDrawNode(IDrawNode task) => -1;
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

            var topologicalDrawNodeOrderRule = new TopologicalDrawNodeOrderRule(Scheduler.Tasks, (x, h) => x.DrawNodeHierarchyChanged += h, (x, h) => x.DrawNodeHierarchyChanged -= h)
            {
                Weight = -10f
            };
            Scheduler.Rules.Add(topologicalDrawNodeOrderRule);
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

            public DelegateController WithParentDrawNode(IDrawNode parentDrawNode)
            {
                foreach (DrawTask controlledTask in ControlledTasks)
                {
                    controlledTask.ParentDrawNode = parentDrawNode;
                }

                return ReturnedController;
            }
        }

        private class TopologicalDrawNodeOrderRule : SchedulingRuleBase<IDrawTask>, IDisposable
        {
            private readonly INotifyCollectionChanged _tasksChanges;
            private readonly Action<IDrawTask, EventHandler> _hierarchyChangeSubscriber;
            private readonly Action<IDrawTask, EventHandler> _hierarchyChangeUnsubscriber;

            public IEnumerable<IDrawTask> Tasks { get; }
            public override bool IsValid => Tasks?.Any() ?? false;

            public TopologicalDrawNodeOrderRule(IEnumerable<IDrawTask> tasks,
                Action<IDrawTask, EventHandler> hierarchyChangeSubscriber,
                Action<IDrawTask, EventHandler> hierarchyChangeUnsubscriber)
            {
                Tasks = tasks;
                _tasksChanges = tasks as INotifyCollectionChanged;

                if (_tasksChanges != null)
                    _tasksChanges.CollectionChanged += OnTasksCollectionChanged;

                _hierarchyChangeSubscriber = hierarchyChangeSubscriber;
                _hierarchyChangeUnsubscriber = hierarchyChangeUnsubscriber;
            }

            public void Dispose()
            {
                if (_tasksChanges != null)
                    _tasksChanges.CollectionChanged -= OnTasksCollectionChanged;
            }

            public override void Apply(ISchedulerGraphBuilder<IDrawTask> graph)
            {
                var rootNodes = new HashSet<IDrawNode>();
                var childrenNodesDictionary = new Dictionary<IDrawNode, HashSet<IDrawNode>>();
                var childrenTasksDictionary = new Dictionary<IDrawNode, HashSet<IDrawTask>>();

                foreach (IDrawTask task in Tasks)
                {
                    IDrawNode parentNode = task.ParentDrawNode;

                    if (!childrenTasksDictionary.TryGetValue(parentNode, out HashSet<IDrawTask> childrenTasks))
                        childrenTasksDictionary[parentNode] = childrenTasks = new HashSet<IDrawTask>();
                    childrenTasks.Add(task);
                    
                    IDrawNode currentNode = task;
                    do
                    {
                        if (!childrenNodesDictionary.TryGetValue(parentNode, out HashSet<IDrawNode> childrenNodes))
                            childrenNodesDictionary[parentNode] = childrenNodes = new HashSet<IDrawNode>();
                        childrenNodes.Add(currentNode);

                        currentNode = parentNode;
                        parentNode = parentNode.ParentDrawNode;
                    }
                    while (parentNode != null);

                    rootNodes.Add(currentNode);
                }
                
                IDrawTask previousTask = null;
                foreach (IDrawNode rootNode in rootNodes)
                {
                    var nodeStack = new Stack<IDrawNode>();
                    nodeStack.Push(rootNode);

                    while (nodeStack.TryPop(out IDrawNode currentNode))
                    {
                        if (childrenTasksDictionary.TryGetValue(currentNode, out HashSet<IDrawTask> tasks))
                        {
                            foreach (IDrawTask currentTask in tasks.OrderBy(currentNode.IndexOfDrawNode))
                            {
                                if (previousTask != null)
                                    graph.TryAddDependency(previousTask, currentTask, this);

                                previousTask = currentTask;
                            }
                        }

                        if (childrenNodesDictionary.TryGetValue(currentNode, out HashSet<IDrawNode> childrenNodes))
                        {
                            foreach (IDrawNode childNode in childrenNodes.OrderByDescending(currentNode.IndexOfDrawNode))
                                nodeStack.Push(childNode);
                        }
                    }
                }
            }

            private void OnTasksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                OnDirty();

                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    foreach (IDrawTask task in Tasks)
                        _hierarchyChangeUnsubscriber?.Invoke(task, OnDirty);
                    return;
                }

                if (e.OldItems != null)
                    foreach (IDrawTask task in e.OldItems)
                        _hierarchyChangeUnsubscriber?.Invoke(task, OnDirty);

                if (e.NewItems != null)
                    foreach (IDrawTask task in e.NewItems)
                        _hierarchyChangeSubscriber?.Invoke(task, OnDirty);
            }
        }
    }
}