using Diese.Injection;
using Glyph.Animation;
using Glyph.Application;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;
using Glyph.Composition.Scheduler.Base;

namespace Glyph.Game
{
    public class GlyphRegistry : DependencyRegistry
    {
        public GlyphRegistry()
        {
            Register<GlyphCompositeInjector>();

            Register<GlyphScheduler<IGlyphComponent, InitializeDelegate>>();
            Register<GlyphScheduler<ILoadContent, LoadContentDelegate>>();
            Register<GlyphScheduler<IUpdate, UpdateDelegate>>();
            Register<GlyphScheduler<IDraw, DrawDelegate>>();

            Register<SchedulerProfile<IGlyphComponent>, GlyphSchedulerProfiles.Initialize>();
            Register<SchedulerProfile<ILoadContent>, GlyphSchedulerProfiles.LoadContent>();
            Register<SchedulerProfile<IUpdate>, GlyphSchedulerProfiles.Update>();
            Register<SchedulerProfile<IDraw>, GlyphSchedulerProfiles.Draw>();

            RegisterFunc<IGlyphComponent, InitializeDelegate>(x => x.Initialize);
            RegisterFunc<ILoadContent, LoadContentDelegate>(x => x.LoadContent);
            RegisterFunc<IUpdate, UpdateDelegate>(x => x.Update);
            RegisterFunc<IDraw, DrawDelegate>(x => x.Draw);

            RegisterFunc<GlyphObject, InitializeDelegate>(x => x.Initialize);
            RegisterFunc<GlyphObject, LoadContentDelegate>(x => x.LoadContent);
            RegisterFunc<GlyphObject, UpdateDelegate>(x => x.Update);
            RegisterFunc<GlyphObject, DrawDelegate>(x => x.Draw);

            Register<SceneNode>();
            Register<Motion>();
        }
    }
}