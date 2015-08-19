using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;
using Glyph.Graphics;

namespace Glyph.Game
{
    public abstract class SceneBase : GlyphSchedulableBase, IScene
    {
        public bool Enabled { get; set; }
        public bool Visible { get; private set; }
        public SceneNode RootNode { get; private set; }

        protected override sealed IGlyphSchedulerAssigner SchedulerAssigner
        {
            get { return Schedulers; }
        }
        
        protected abstract SchedulerHandlerBase Schedulers { get; }
        protected abstract ISceneRenderer SceneRenderer { get; }

        protected SceneBase(IDependencyInjector injector)
            : base(injector)
        {
            Enabled = true;
            Visible = true;

            RootNode = Add<SceneNode>();
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

            SceneRenderer.RenderingProcess();
        }

        public virtual void PreDraw()
        {
        }

        public virtual void PostDraw()
        {
        }

        public class SchedulerHandlerBase : GlyphSchedulerHandler
        {
            public IGlyphScheduler<IGlyphComponent, InitializeDelegate> Initialize { get; private set; }
            public IGlyphScheduler<ILoadContent, LoadContentDelegate> LoadContent { get; private set; }
            public IGlyphScheduler<IUpdate, UpdateDelegate> Update { get; private set; }

            public SchedulerHandlerBase(IDependencyInjector injector)
                : base(injector)
            {
                Initialize = Add<IGlyphComponent, InitializeDelegate>();
                LoadContent = Add<ILoadContent, LoadContentDelegate>();
                Update = Add<IUpdate, UpdateDelegate>();
            }
        }
    }
}