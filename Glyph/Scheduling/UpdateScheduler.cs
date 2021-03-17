using Glyph.Scheduling.Base;

namespace Glyph.Scheduling
{
    public delegate void UpdateDelegate(ElapsedTime elapsedTime);

    public interface IUpdateTask
    {
        bool Active { get; }
        void Update(ElapsedTime elapsedTime);
    }

    public class UpdateTask : DelegateTaskBase<UpdateDelegate>, IUpdateTask
    {
        public UpdateTask(UpdateDelegate taskDelegate)
            : base(taskDelegate) { }

        public bool Active => true;
        public void Update(ElapsedTime elapsedTime) => TaskDelegate(elapsedTime);
    }

    public class UpdateScheduler : GlyphScheduler<IUpdateTask, UpdateDelegate>
    {
        public UpdateScheduler()
            : base(x => new UpdateTask(x)) { }
    }
}