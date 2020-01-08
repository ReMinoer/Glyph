using System;
using Niddle;
using Fingear.MonoGame;
using Glyph.Animation;
using Glyph.Animation.Motors;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Composition.Messaging;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Inputs;
using Glyph.Core.Layers;
using Glyph.Core.Resolvers;
using Glyph.Core.Scheduler;
using Glyph.Core.Tracking;
using Glyph.Graphics;
using Glyph.Graphics.Particles;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Messaging;
using Glyph.Particles;
using Glyph.Physics;
using Glyph.Resolver;
using Glyph.Scripting;
using Glyph.Tools;
using Glyph.Tools.Brushing.Controllers;
using Glyph.Tools.ShapeRendering;
using Glyph.UI;
using Glyph.UI.Menus;
using Glyph.UI.Simple;
using Niddle.Dependencies.Builders;
using Taskete;

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

                #region Composition & Scheduling

                Type<GlyphScheduler<IGlyphComponent, InitializeDelegate>>(),
                Type<GlyphScheduler<ILoadContent, LoadContentDelegate>>(),
                Type<GlyphScheduler<IUpdate, UpdateDelegate>>(),
                Type<GlyphScheduler<IDraw, DrawDelegate>>(),

                Type<IReadOnlyScheduler<Predicate<object>>>().Keyed(typeof(IGlyphComponent)).Using(GlyphSchedulerProfiles.Instance.Initialize),
                Type<IReadOnlyScheduler<Predicate<object>>>().Keyed(typeof(ILoadContent)).Using(GlyphSchedulerProfiles.Instance.LoadContent),
                Type<IReadOnlyScheduler<Predicate<object>>>().Keyed(typeof(IUpdate)).Using(GlyphSchedulerProfiles.Instance.Update),
                Type<IReadOnlyScheduler<Predicate<object>>>().Keyed(typeof(IDraw)).Using(GlyphSchedulerProfiles.Instance.Draw),

                Type<Func<IGlyphComponent, InitializeDelegate>>().Using(x => x.Initialize),
                Type<Func<ILoadContent, LoadContentDelegate>>().Using(x => x.LoadContent),
                Type<Func<IUpdate, UpdateDelegate>>().Using(x => x.Update),
                Type<Func<IDraw, DrawDelegate>>().Using(x => x.Draw),

                Type<Func<GlyphObject, InitializeDelegate>>().Using(x => x.Initialize),
                Type<Func<GlyphObject, LoadContentDelegate>>().Using(x => x.LoadContent),
                Type<Func<GlyphObject, UpdateDelegate>>().Using(x => x.Update),
                Type<Func<GlyphObject, DrawDelegate>>().Using(x => x.Draw),

                Type<GlyphObject>(),

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

                Generic(typeof(LayerRoot<>)),
                Generic(typeof(ILayerRoot<>)).LinkedTo(typeof(LayerRoot<>)),

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

                Generic(typeof(MappingRenderer<>)),

                #endregion

                #region Audio
                
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

                Type<ColliderComposite>(),

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

                Generic(typeof(ShapedComponentRendererManager<>)),

                Type<RectangleComponentRenderer>(),
                Type<CircleComponentRenderer>(),

                Type<FreeCamera>(),
                Type<SceneNodeEditor>(),
                Type<PositionHandle>(),
                Type<ShapedObjectSelector>(),

                Type<InputLogger>(),
                Type<ControlLogger>(),

                Type<EngineCursorBrushController>(),
                Type<DataCursorBrushController>(),

                #endregion

                #region UI

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