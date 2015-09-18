using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;
using Glyph.Graphics;

namespace Glyph.Game
{
    public abstract class SceneBase<TSchedulerHandler> : GlyphSchedulableBase, IScene
        where TSchedulerHandler : SceneSchedulerHandlerBase
    {
        public bool Enabled { get; set; }
        public bool Visible { get; private set; }
        public abstract SceneNode RootNode { get; }

        protected override sealed IGlyphSchedulerAssigner SchedulerAssigner
        {
            get { return Schedulers; }
        }

        protected abstract TSchedulerHandler Schedulers { get; }
        protected abstract ISceneRenderer Renderer { get; }

        protected SceneBase(IDependencyInjector injector)
            : base(injector)
        {
            Enabled = true;
            Visible = true;
        }

        public override void Initialize()
        {
            foreach (InitializeDelegate initialize in Schedulers.Initialize.TopologicalOrder)
                initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (LoadContentDelegate loadContent in Schedulers.LoadContent.TopologicalOrder)
                loadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (UpdateDelegate update in Schedulers.Update.TopologicalOrder)
                update(elapsedTime);
        }

        public void Draw()
        {
            if (!Visible)
                return;

            Renderer.RenderingProcess();
        }

        public virtual void PreDraw()
        {
        }

        public virtual void PostDraw()
        {
        }
    }
}