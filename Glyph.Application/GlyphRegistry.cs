using System;
using Niddle;
using Fingear.MonoGame;
using Glyph.Animation;
using Glyph.Animation.Motors;
using Glyph.Animation.Parallax;
using Glyph.Audio;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Inputs;
using Glyph.Core.Resolvers;
using Glyph.Core.Schedulers;
using Glyph.Core.Tracking;
using Glyph.Graphics;
using Glyph.Graphics.Particles;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Messaging;
using Glyph.Particles;
using Glyph.Physics;
using Glyph.Resolver;
using Glyph.Scheduling;
using Glyph.Scripting;
using Glyph.Tools;
using Glyph.Tools.Brushing.Controllers;
using Glyph.Tools.Brushing.Decorators.Cursors;
using Glyph.Tools.Parallax;
using Glyph.Tools.Transforming;
using Glyph.UI;
using Glyph.UI.Menus;
using Glyph.UI.Simple;
using Niddle.Dependencies.Builders;

namespace Glyph.Application
{
    static public class GlyphRegistry
    {
        static public DependencyRegistry BuildGlobalRegistry()
        {
            var registry = new DependencyRegistry
            {
                #region Resolvers

                Type<IDependencyRegistry>().Keyed(ResolverScope.Global).LinkedTo<IDependencyRegistry>(),
                Type<IDependencyRegistry>().Keyed(ResolverScope.Local).Creating(BuildLocalRegistry),

                Type<RegistryResolver>().Keyed(ResolverScope.Global).LinkedTo<RegistryResolver>(),
                Type<IDependencyResolver>().LinkedTo<RegistryResolver>(),
                Type<IDependencyResolver>().Keyed(ResolverScope.Global).LinkedTo<RegistryResolver>(),

                Type<GlyphCompositeDependencyResolver>(),
                Type<GlyphResolveContext>(),

                #endregion

                #region Composition

                Type<GlyphObject>(),

                #endregion

                #region Scheduling
                
                Type<InitializeScheduler>().Creating(InitializeScheduler),
                Type<LoadContentScheduler>().Creating(LoadContentScheduler),
                Type<UpdateScheduler>().Creating(UpdateScheduler).AsSingleton(),
                Type<DrawScheduler>().AsSingleton(),
                Type<RenderScheduler>(),

                Type<InitializeComponentScheduler>(),
                Type<LoadContentComponentScheduler>(),
                Type<UpdateComponentScheduler>(),

                #endregion

                #region Messaging
                
                Type<TrackingRouter>().AsSingleton(),
                Type<ITrackingRouter>().LinkedTo<TrackingRouter>(),
                Type<ITrackingRouter>().Keyed(ResolverScope.Global).LinkedTo<TrackingRouter>(),
                Type<ISubscribableRouter>().LinkedTo<TrackingRouter>(),
                Type<ISubscribableRouter>().Keyed(ResolverScope.Global).LinkedTo<TrackingRouter>(),
                Type<IRouter>().LinkedTo<TrackingRouter>(),
                Type<IRouter>().Keyed(ResolverScope.Global).LinkedTo<TrackingRouter>(),

                Generic(typeof(Receiver<>)),
                Generic(typeof(MessagingTracker<>)),
                Generic(typeof(MessagingSpace<>)),

                #endregion

                #region Core

                Type<SceneNode>(),
                Type<AnchoredSceneNode>(),
                Type<PositionBinding>(),

                Type<Camera>(),
                Type<TargetView>(),
                Type<FillView>(),
                Type<UniformFillTargetView>(),

                Type<InteractiveRoot>(),
                Type<Controls>(),

                Type<Flipper>(),

                #endregion

                #region Animation

                Type<Motion>(),
                Type<LinearMotor>(),
                Type<SteeringMotor>(),
                Type<SeekingMotor>(),
                Type<TrackMotor>(),
                Type<TimedTrajectoryMotor>(),
                Type<MeasurableTrajectoryMotor>(),
                Generic(typeof(AnimationGraph<,>)),
                Generic(typeof(AnimationPlayer<>)),

                Type<ParallaxRoot>(),
                Type<ParallaxLayer>(),

                #endregion

                #region Particles
                
                Type<ParticleEmitter>(),
                Type<StandardParticle>(),

                #endregion

                #region Graphics

                Type<SpriteLoader>(),
                Type<SpriteTransformer>(),
                Type<SpriteRenderer>(),

                Type<SpriteSheet>(),
                Type<SpriteSheetSplit>(),
                Type<SpriteAnimator>(),

                Type<SpriteArea>(),

                Type<RectangleSprite>(),
                Type<FilledRectangleSprite>(),
                Type<CircleSprite>(),
                Type<FilledCircleSprite>(),

                Type<FillingRectangle>(),
                Type<FillingRenderer>(),
                Type<TexturingRenderer>(),

                Type<MeshRenderer>(),
                Type<MeshesComponent>(),

                Type<EffectLoader>(),

                Generic(typeof(MappingRenderer<>)),

                #endregion

                #region Audio
                
                Type<SongPlayer>(),
                Type<SoundLoader>(),
                Type<SoundEmitter>(),
                Type<SoundListener>(),
                Type<SoundListenerManager>(),

                #endregion

                #region Physics

                Type<PhysicsManager>().AsSingleton(),
                Type<ColliderManager>().AsSingleton(),

                Type<RectangleCollider>(),
                Type<CircleCollider>(),
                Generic(typeof(GridCollider<>)),

                #endregion

                #region Scripting

                Type<TriggerManager>().AsSingleton(),
                Type<Trigger>(),
                Type<Actor>(),

                #endregion

                #region Input
                
                Type<InputSystem>().Using(InputSystem.Instance),

                #endregion

                #region Tools

                Type<FreeCamera>(),

                Type<TransformationEditor>(),
                Type<MultiModeTransformationEditor>(),
                Type<AdvancedPositionHandle>(),
                Type<AdvancedRotationHandle>(),
                Type<AdvancedScaleHandle>(),
                Type<SimplePositionHandle>(),

                Type<RectangleEditor>(),
                Type<AdvancedRectanglePositionHandle>(),
                Type<AdvancedRectangleBorderPositionHandle>(),

                Type<ShapedObjectSelector>(),

                Type<InputLogger>(),
                Type<ControlLogger>(),

                Type<EngineCursorBrushController>(),
                Type<DataCursorBrushController>(),
                Type<GridCursor>(),

                Type<ParallaxManipulator>(),

                #endregion

                #region UI

                Type<InterfaceRoot>(),
                Type<UserInterface>(),

                Type<Text>(),

                Type<SimpleBorder>(),
                Type<SimpleFrame>(),
                Type<SimpleButton>(),

                Type<LinearMenu>()

                #endregion
            };

            registry.Add(Type<IDependencyRegistry>().Using(registry));
            return registry;
        }

        static private InitializeScheduler InitializeScheduler() => DefaultGlyphSchedulerRules.Setup(new InitializeScheduler());
        static private LoadContentScheduler LoadContentScheduler() => DefaultGlyphSchedulerRules.Setup(new LoadContentScheduler());
        static private UpdateScheduler UpdateScheduler() => DefaultGlyphSchedulerRules.Setup(new UpdateScheduler());

        static public DependencyRegistry BuildLocalRegistry()
        {
            return new DependencyRegistry
            {
                Type<ITrackingRouter>().LinkedTo<TrackingRouter>(),
                Type<ITrackingRouter>().Keyed(ResolverScope.Local).LinkedTo<TrackingRouter>(),
                Type<ISubscribableRouter>().LinkedTo<TrackingRouter>(),
                Type<ISubscribableRouter>().Keyed(ResolverScope.Local).LinkedTo<TrackingRouter>(),
                Type<IRouter>().LinkedTo<TrackingRouter>(),
                Type<IRouter>().Keyed(ResolverScope.Local).LinkedTo<TrackingRouter>(),
            };
        }
        
        static private ITypeDependencyBuilder<T> Type<T>() => GlyphDependency.OnType<T>();
        static private IGenericDependencyBuilder Generic(Type typeDefinition) => GlyphDependency.OnGeneric(typeDefinition);
    }

}