using Diese.Injection;
using Glyph.Animation;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Injection;
using Glyph.Composition.Layers;
using Glyph.Composition.Messaging;
using Glyph.Composition.Scheduler;
using Glyph.Composition.Scheduler.Base;
using Glyph.Composition.Tracking;
using Glyph.Graphics;
using Glyph.Graphics.Particles;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Messaging;
using Glyph.Particles;
using Glyph.Physics;
using Glyph.Physics.Colliders;
using Glyph.Scripting.Triggers;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Glyph.UI;
using Glyph.UI.Menus;
using Glyph.UI.Simple;
using SongPlayer = Glyph.Audio.SongPlayer;

namespace Glyph.Application
{
    public class GlyphRegistry : DependencyRegistry
    {
        public GlyphRegistry()
        {
            #region Injection

            RegisterInstance<IDependencyRegistry>(this);
            Link<IDependencyRegistry, IDependencyRegistry>(null, InjectionScope.Global);

            Register<GlyphCompositeInjector>();
            Register<IDependencyRegistry, LocalGlyphRegistry>(Subsistence.Singleton, InjectionScope.Local);

            #endregion

            #region Composition & Scheduling

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

            #endregion

            #region Messaging

            RegisterGeneric(typeof(Receiver<>));
            RegisterGeneric(typeof(GlobalRouter<>), Subsistence.Singleton);
            LinkGeneric(typeof(IRouter<>), typeof(GlobalRouter<>));
            LinkGeneric(typeof(IRouter<>), typeof(GlobalRouter<>), null, InjectionScope.Global);
            RegisterGeneric(typeof(MessagingTracker<>));

            #endregion

            #region Fundamental components

            Register<SceneNode>();
            RegisterGeneric(typeof(LayerRoot<>));
            LinkGeneric(typeof(ILayerRoot<>), typeof(LayerRoot<>));

            #endregion

            #region Animation

            Register<Motion>();
            RegisterGeneric(typeof(Animator<>));
            RegisterGeneric(typeof(AnimationPlayer<>));

            #endregion

            #region Particles
            
            Register<ParticleEmitter>();
            Register<StandardParticle>();

            #endregion

            #region Graphics

            Register<SpriteLoader>();
            Register<SpriteTransformer>();
            Register<SpriteRenderer>();

            Register<SpriteSheet>();
            Register<SpriteSheetSplit>();
            Register<SpriteAnimator>();

            Register<SpriteArea>();

            Register<RectangleSprite>();
            Register<FilledRectangleSprite>();
            Register<CircleSprite>();

            Register<FillingRectangle>();
            Register<FillingRenderer>();
            Register<TexturingRenderer>();

            RegisterGeneric(typeof(MappingRenderer<>));

            #endregion

            #region Audio

            Register<SongPlayer>();
            Register<SoundListener>();
            Register<SoundEmitter>();

            #endregion

            #region Physics

            Register<PhysicsManager>(Subsistence.Singleton);
            Register<ColliderManager>(Subsistence.Singleton);

            Register<RectangleCollider>();
            Register<CircleCollider>();
            RegisterGeneric(typeof(GridCollider<>));

            Register<ColliderComposite>();

            #endregion

            #region Scripting

            Register<TriggerArea>();

            #endregion

            #region Tools

            RegisterGeneric(typeof(ShapeRendererManager<>));

            Register<RectangleShapeRenderer>();
            Register<CircleShapeRenderer>();

            RegisterGeneric(typeof(MapEditor<>));

            #endregion

            #region UI

            Register<Text>();

            Register<SimpleBorder>();
            Register<SimpleFrame>();
            Register<SimpleButton>();

            Register<LinearMenu>();

            #endregion
        }
    }
}