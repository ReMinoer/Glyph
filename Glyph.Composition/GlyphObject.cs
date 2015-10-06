using Diese.Injection;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Composition
{
    public class GlyphObject : GlyphSchedulableBase, IEnableable, ILoadContent, IUpdate, IDraw
    {
        protected readonly SchedulerHandler Schedulers;
        private bool _initialized;
        private bool _contentLoaded;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        protected override sealed IGlyphSchedulerAssigner SchedulerAssigner
        {
            get { return Schedulers; }
        }

        public GlyphObject(IDependencyInjector injector)
            : base(injector)
        {
            Enabled = true;
            Visible = true;

            Schedulers = new SchedulerHandler(injector);
        }

        public override sealed void Add(IGlyphComponent item)
        {
            base.Add(item);

            if (!_initialized)
                item.Initialize();

            if (!_contentLoaded)
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

            foreach (DrawDelegate draw in Schedulers.Draw.TopologicalOrder)
                draw(spriteBatch);
        }

        protected class SchedulerHandler : GlyphSchedulerHandler
        {
            public IGlyphScheduler<IGlyphComponent, InitializeDelegate> Initialize { get; private set; }
            public IGlyphScheduler<ILoadContent, LoadContentDelegate> LoadContent { get; private set; }
            public IGlyphScheduler<IUpdate, UpdateDelegate> Update { get; private set; }
            public IGlyphScheduler<IDraw, DrawDelegate> Draw { get; private set; }

            public SchedulerHandler(IDependencyInjector injector)
                : base(injector)
            {
                Initialize = Add<IGlyphComponent, InitializeDelegate>();
                LoadContent = Add<ILoadContent, LoadContentDelegate>();
                Update = Add<IUpdate, UpdateDelegate>();
                Draw = Add<IDraw, DrawDelegate>();
            }
        }
    }
}