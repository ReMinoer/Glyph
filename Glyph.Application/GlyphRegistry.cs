using System;
using Diese.Injection;
using Diese.Scheduling;
using Fingear.MonoGame;
using Glyph.Animation;
using Glyph.Animation.Motors;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Messaging;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Injection;
using Glyph.Core.Inputs;
using Glyph.Core.Layers;
using Glyph.Core.Scheduler;
using Glyph.Core.Tracking;
using Glyph.Graphics;
using Glyph.Graphics.Particles;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Messaging;
using Glyph.Particles;
using Glyph.Physics;
using Glyph.Scripting;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Glyph.UI;
using Glyph.UI.Menus;
using Glyph.UI.Simple;

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

            RegisterInstance(GlyphSchedulerProfiles.Instance.Initialize, typeof(IGlyphComponent));
            RegisterInstance(GlyphSchedulerProfiles.Instance.LoadContent, typeof(ILoadContent));
            RegisterInstance(GlyphSchedulerProfiles.Instance.Update, typeof(IUpdate));
            RegisterInstance(GlyphSchedulerProfiles.Instance.Draw, typeof(IDraw));

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

            #region Core

            Register<SceneNode>();

            RegisterGeneric(typeof(LayerRoot<>));
            LinkGeneric(typeof(ILayerRoot<>), typeof(LayerRoot<>));

            Register<Camera>();
            Register<View>();

            Register<Controls>();

            Register<Flipper>();

            #endregion

            #region Animation

            Register<Motion>();
            Register<LinearMotor>();
            Register<SteeringMotor>();
            Register<TimedTrajectoryMotor>();
            Register<MeasurableTrajectoryMotor>();
            RegisterGeneric(typeof(AnimationGraph<,>));
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
            Register<FilledCircleSprite>();

            Register<FillingRectangle>();
            Register<FillingRenderer>();
            Register<TexturingRenderer>();

            RegisterGeneric(typeof(MappingRenderer<>));

            #endregion

            #region Audio
            
            Register<SoundLoader>();
            Register<SoundEmitter>();
            Register<SoundListener>();
            Register<SoundListenerManager>();

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

            Register<TriggerManager>(Subsistence.Singleton);
            Register<Trigger>();
            Register<Actor>();

            #endregion

            #region Input
            
            RegisterInstance<InputSystem>(InputSystem.Instance);

            #endregion

            #region Tools

            RegisterGeneric(typeof(ShapedComponentRendererManager<>));

            Register<RectangleComponentRenderer>();
            Register<CircleComponentRenderer>();

            Register<ShapedObjectSelector>();

            Register<InputLogger>();
            Register<ControlLogger>();

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