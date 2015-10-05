using Diese.Injection;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Composition
{
    // TASK : Schedule methods with attributes
    public class GlyphObject : GlyphSchedulableBase, IEnableable, ILoadContent, IUpdate, IDraw
    {
        protected readonly SchedulerHandler Schedulers;
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