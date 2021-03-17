using Glyph.Scheduling.Base;

namespace Glyph.Scheduling
{
    public delegate void RenderDelegate(IDrawer drawer);

    public interface IRenderTask
    {
        void Render(IDrawer drawer);
    }

    public class RenderTask : DelegateTaskBase<RenderDelegate>, IRenderTask
    {
        public RenderTask(RenderDelegate taskDelegate)
            : base(taskDelegate) { }
        
        public void Render(IDrawer drawer) => TaskDelegate(drawer);
    }

    public class RenderScheduler : GlyphScheduler<IRenderTask, RenderDelegate>
    {
        public IRenderTask RenderViewTask { get; }

        public RenderScheduler()
            : base(x => new RenderTask(x))
        {
            RenderViewTask = new RenderTask(drawer => drawer.RenderView());
            Scheduler.Tasks.Add(RenderViewTask);
        }
    }
}