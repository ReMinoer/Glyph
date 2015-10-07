using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;
using Glyph.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Game
{
    public abstract class SceneBase<TSchedulerHandler> : GlyphSchedulableBase, IScene
        where TSchedulerHandler : SceneSchedulerHandlerBase
    {
        private bool _initialized;
        private bool _contentLoaded;
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

        public override sealed void Add(IGlyphComponent item)
        {
            base.Add(item);

            if (_initialized)
                item.Initialize();

            if (_contentLoaded)
            {
                var loadContent = item as ILoadContent;
                if (loadContent != null)
                    loadContent.LoadContent(Injector.Resolve<ContentLibrary>());
            }
        }

        public override void Initialize()
        {
            foreach (InitializeDelegate initialize in Schedulers.Initialize.TopologicalOrder)
                initialize();

            _initialized = true;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (LoadContentDelegate loadContent in Schedulers.LoadContent.TopologicalOrder)
                loadContent(contentLibrary);

            _contentLoaded = true;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (UpdateDelegate update in Schedulers.Update.TopologicalOrder)
            {
                if (!Enabled)
                    return;

                update(elapsedTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            Renderer.RenderingProcess(spriteBatch);
        }

        public virtual void PreDraw(SpriteBatch spriteBatch)
        {
        }

        public virtual void PostDraw()
        {
        }
    }
}