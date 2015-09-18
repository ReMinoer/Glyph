using Diese.Injection;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;

namespace Glyph.Game
{
    public class SceneSchedulerHandlerBase : GlyphSchedulerHandler
    {
        public IGlyphScheduler<IGlyphComponent, InitializeDelegate> Initialize { get; private set; }
        public IGlyphScheduler<ILoadContent, LoadContentDelegate> LoadContent { get; private set; }
        public IGlyphScheduler<IUpdate, UpdateDelegate> Update { get; private set; }

        public SceneSchedulerHandlerBase(IDependencyInjector injector)
            : base(injector)
        {
            Initialize = Add<IGlyphComponent, InitializeDelegate>();
            LoadContent = Add<ILoadContent, LoadContentDelegate>();
            Update = Add<IUpdate, UpdateDelegate>();
        }
    }
}