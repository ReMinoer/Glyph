using Diese.Injection;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler.Base;

namespace Glyph.Composition.Scheduler
{
    public class GlyphObjectScheduler
    {
        public GlyphScheduler<IGlyphComponent, InitializeDelegate> Initialize { get; private set; }
        public GlyphScheduler<ILoadContent, LoadContentDelegate> LoadContent { get; private set; }
        public GlyphScheduler<IUpdate, UpdateDelegate> Update { get; private set; }
        public GlyphScheduler<IHandleInput, HandleInputDelegate> HandleInput { get; private set; }
        public GlyphScheduler<IDraw, DrawDelegate> Draw { get; private set; }

        public GlyphObjectScheduler(IDependencyInjector injector)
        {
            Initialize = new GlyphScheduler<IGlyphComponent, InitializeDelegate>(x => x.Initialize);
            LoadContent = new GlyphScheduler<ILoadContent, LoadContentDelegate>(x => x.LoadContent);
            Update = new GlyphScheduler<IUpdate, UpdateDelegate>(x => x.Update);
            HandleInput = new GlyphScheduler<IHandleInput, HandleInputDelegate>(x => x.HandleInput);
            Draw = new GlyphScheduler<IDraw, DrawDelegate>(x => x.Draw);

            Initialize.ApplyProfile(injector.Resolve<SchedulerProfile<IGlyphComponent>>());
            LoadContent.ApplyProfile(injector.Resolve<SchedulerProfile<ILoadContent>>());
            Update.ApplyProfile(injector.Resolve<SchedulerProfile<IUpdate>>());
            HandleInput.ApplyProfile(injector.Resolve<SchedulerProfile<IHandleInput>>());
            Draw.ApplyProfile(injector.Resolve<SchedulerProfile<IDraw>>());
        }

        public void BatchStart()
        {
            Initialize.BatchStart();
            LoadContent.BatchStart();
            Update.BatchStart();
            HandleInput.BatchStart();
            Draw.BatchStart();
        }

        public void BatchEnd()
        {
            Initialize.BatchEnd();
            LoadContent.BatchEnd();
            Update.BatchEnd();
            HandleInput.BatchEnd();
            Draw.BatchEnd();
        }

        internal void Add(IGlyphComponent item)
        {
            Initialize.Add(item.Initialize);

            var loadContent = item as ILoadContent;
            if (loadContent != null)
                LoadContent.Add(loadContent.LoadContent);

            var update = item as IUpdate;
            if (update != null)
                Update.Add(update.Update);

            var handleInput = item as IHandleInput;
            if (handleInput != null)
                HandleInput.Add(handleInput.HandleInput);

            var draw = item as IDraw;
            if (draw != null)
                Draw.Add(draw.Draw);
        }

        internal void Add(GlyphObject item)
        {
            Initialize.Add(item.Initialize);
            LoadContent.Add(item.LoadContent);
            Update.Add(item.Update);
            HandleInput.Add(item.HandleInput);
            Draw.Add(item.Draw);
        }

        internal void Remove(IGlyphComponent item)
        {
            Initialize.Remove(item.Initialize);

            var loadContent = item as ILoadContent;
            if (loadContent != null)
                LoadContent.Remove(loadContent.LoadContent);

            var update = item as IUpdate;
            if (update != null)
                Update.Remove(update.Update);

            var handleInput = item as IHandleInput;
            if (handleInput != null)
                HandleInput.Remove(handleInput.HandleInput);

            var draw = item as IDraw;
            if (draw != null)
                Draw.Remove(draw.Draw);
        }

        internal void Remove(GlyphObject item)
        {
            Initialize.Remove(item.Initialize);
            LoadContent.Remove(item.LoadContent);
            Update.Remove(item.Update);
            HandleInput.Remove(item.HandleInput);
            Draw.Remove(item.Draw);
        }

        internal void Clear()
        {
            Initialize.Clear();
            LoadContent.Clear();
            Update.Clear();
            HandleInput.Clear();
            Draw.Clear();
        }
    }
}