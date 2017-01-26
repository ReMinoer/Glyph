﻿using Diese.Injection;
using Fingear;
using Fingear.MonoGame;
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
using Glyph.Core;
using Glyph.Core.Colliders;
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

            #region Core

            Register<SceneNode>();

            RegisterGeneric(typeof(LayerRoot<>));
            LinkGeneric(typeof(ILayerRoot<>), typeof(LayerRoot<>));

            Register<Camera>();
            Register<View>();

            #endregion

            #region Animation

            Register<Motion>();
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
            
            RegisterInstance<InputSystem>(MonoGameInputSytem.Instance);

            #endregion

            #region Tools

            RegisterGeneric(typeof(ShapedComponentRendererManager<>));

            Register<RectangleComponentRenderer>();
            Register<CircleComponentRenderer>();

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