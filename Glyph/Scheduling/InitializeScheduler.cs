using Glyph.Scheduling.Base;

namespace Glyph.Scheduling
{
    public delegate void InitializeDelegate();

    public interface IInitializeTask
    {
        void Initialize();
    }

    public class InitializeTask : DelegateTaskBase<InitializeDelegate>, IInitializeTask
    {
        public InitializeTask(InitializeDelegate taskDelegate)
            : base(taskDelegate) { }

        public void Initialize() => TaskDelegate();
    }

    public class InitializeScheduler : GlyphScheduler<IInitializeTask, InitializeDelegate>
    {
        public InitializeScheduler()
            : base(x => new InitializeTask(x)) { }
    }
}