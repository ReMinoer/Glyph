using Glyph.Scheduling.Base;

namespace Glyph.Scheduling
{
    public delegate void DrawDelegate(IDrawer drawer);

    public interface IDrawTask
    {
        void Draw(IDrawer drawer);
    }

    public class DrawTask : DelegateTaskBase<DrawDelegate>, IDrawTask
    {
        public DrawTask(DrawDelegate taskDelegate)
            : base(taskDelegate) { }

        public void Draw(IDrawer drawer) => TaskDelegate(drawer);
    }

    public class DrawScheduler : GlyphScheduler<IDrawTask, DrawDelegate>
    {
        public DrawScheduler()
            : base(x => new DrawTask(x)) { }
    }
}