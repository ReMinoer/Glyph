using Diese.Injection;
using Glyph.Animation;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Injection;
using Glyph.Composition.Messaging;
using Glyph.Composition.Scheduler;
using Glyph.Composition.Scheduler.Base;
using Glyph.Graphics;
using Glyph.Graphics.Particles;
using Glyph.Messaging;
using Glyph.Physics;
using Glyph.Physics.Colliders;

namespace Glyph.Application
{
    public class GlyphRegistry : DependencyRegistry
    {
        public GlyphRegistry()
        {
            RegisterInstance<IDependencyRegistry>(this);

            Register<GlyphCompositeInjector>();
            Register<IDependencyRegistry, LocalGlyphRegistry>(Subsistence.Singleton, InjectionScope.Local);

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

            Register<GlyphObject>();

            RegisterGeneric(typeof(GlobalRouter<>), Subsistence.Singleton);
            RegisterGeneric(typeof(Receiver<>));
            LinkGeneric(typeof(IRouter<>), typeof(GlobalRouter<>));

            Register<SceneNode>();
            Register<Motion>();
            RegisterGeneric(typeof(Animator<>));
            RegisterGeneric(typeof(StandardAnimationPlayer<>));

            Register<SpriteLoader>();
            Register<SpriteSheet>();
            Register<SpriteSheetSplit>();
            Register<SpriteTransformer>();
            Register<SpriteAnimator>();
            Register<SpriteRenderer>();
            Register<ParticleEmitter>();

            Register<SongPlayer>();
            Register<SoundListener>();
            Register<SoundEmitter>();

            Register<PhysicsManager>(Subsistence.Singleton);
            Register<ColliderManager>(Subsistence.Singleton);
            Register<RectangleCollider>();
            Register<CircleCollider>();
            Register<ColliderBase.Context>();
        }
    }
}