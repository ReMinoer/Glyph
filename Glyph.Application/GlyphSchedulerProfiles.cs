using System;
using Glyph.Animation;
using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories.Players;
using Glyph.Audio;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Layers;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Renderer.Base;
using Glyph.Graphics.Shapes;
using Glyph.Particles;
using Glyph.Scripting;
using Taskete;

namespace Glyph.Application
{
    public class GlyphSchedulerProfiles
    {
        static private GlyphSchedulerProfiles _instance;
        static public GlyphSchedulerProfiles Instance => _instance ?? (_instance = new GlyphSchedulerProfiles());

        public IReadOnlyScheduler<Predicate<object>> Initialize { get; }
        public IReadOnlyScheduler<Predicate<object>> LoadContent { get; }
        public IReadOnlyScheduler<Predicate<object>> Update { get; }
        public IReadOnlyScheduler<Predicate<object>> Draw { get; }

        private GlyphSchedulerProfiles()
        {
            var initialize = new Scheduler<Predicate<object>>();
            initialize.Plan(x => x is SceneNode);
            initialize.Plan(x => x is PositionBinding);
            initialize.Plan(x => x is ILayerRoot);
            initialize.Plan(x => x is ITrajectoryPlayer);
            initialize.Plan(x => x is MotorBase);
            initialize.Plan(x => x is Motion);

            initialize.Plan(x => x is ISpriteSource);
            initialize.Plan(x => x is SpriteTransformer);
            initialize.Plan(x => x is SpriteAnimator);
            initialize.Plan(x => x is RendererBase);

            initialize.Plan(x => x is SongPlayer);
            initialize.Plan(x => x is SoundListener);
            initialize.Plan(x => x is SoundEmitter);

            initialize.Plan(x => x is ParticleEmitter);

            initialize.Plan(x => x is ICollider);
            initialize.Plan(x => x is SpriteArea);

            initialize.Plan(x => x is Trigger);
            initialize.Plan(x => x is Actor);
            
            var loadContent = new Scheduler<Predicate<object>>();
            loadContent.Plan(x => x is SpriteLoader);
            loadContent.Plan(x => x is ShapedSpriteBase);
            loadContent.Plan(x => x is SongPlayer);

            var update = new Scheduler<Predicate<object>>();
            update.Plan(x => x is PositionBinding);
            update.Plan(x => x is ITrajectoryPlayer);
            update.Plan(x => x is MotorBase);
            update.Plan(x => x is Motion);

            update.Plan(x => x is SpriteAnimator);
            update.Plan(x => x is SongPlayer);
            update.Plan(x => x is ICollider);
            update.Plan(x => x is ParticleEmitter);
            update.Plan(x => x is Actor);

            var draw = new Scheduler<Predicate<object>>();
            draw.Plan(x => x is RendererBase);
            draw.Plan(x => x is ParticleEmitter);

            Initialize = new ReadOnlyScheduler<Predicate<object>>(initialize);
            LoadContent = new ReadOnlyScheduler<Predicate<object>>(loadContent);
            Update = new ReadOnlyScheduler<Predicate<object>>(update);
            Draw = new ReadOnlyScheduler<Predicate<object>>(draw);
        }
    }
}