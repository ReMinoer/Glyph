using System;
using System.Collections.Generic;
using Glyph.Animation;
using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories.Players;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Layers;
using Glyph.Core.Scheduler;
using Glyph.Graphics;
using Glyph.Graphics.Renderer.Base;
using Glyph.Graphics.Shapes;
using Glyph.Particles;
using Glyph.Scripting;

namespace Glyph.Application
{
    static public class DefaultGlyphSchedulerRules
    {
        static public void Setup(GlyphScheduler<IInitializeTask> scheduler)
        {
            AddSequence(scheduler)

                .BeginWith<SceneNode>()
                .Then<PositionBinding>()
                .Then<ILayerRoot>()
                .Then<ITrajectoryPlayer>()
                .Then<MotorBase>()
                .Then<Motion>()

                .Then<ISpriteSource>()
                .Then<SpriteTransformer>()
                .Then<SpriteAnimator>()
                .Then<RendererBase>()

                .Then<SongPlayer>()
                .Then<SoundListener>()
                .Then<SoundEmitter>()

                .Then<ParticleEmitter>()

                .Then<ICollider>()
                .Then<SpriteArea>()

                .Then<Trigger>()
                .Then<Actor>();
        }

        static public void Setup(AsyncGlyphScheduler<ILoadContentTask, IContentLibrary> scheduler)
        {
            AddSequence(scheduler)

                .BeginWith<SpriteLoader>()
                .Then<ShapedSpriteBase>()
                .Then<SongPlayer>();
        }

        static public void Setup(GlyphScheduler<IUpdateTask> scheduler)
        {
            AddSequence(scheduler)

                .BeginWith<PositionBinding>()
                .Then<ITrajectoryPlayer>()
                .Then<MotorBase>()
                .Then<Motion>()

                .Then<SpriteAnimator>()
                .Then<SongPlayer>()
                .Then<ICollider>()
                .Then<ParticleEmitter>()
                .Then<Actor>();
        }

        static public void Setup(GlyphScheduler<IDrawTask> scheduler)
        {
            AddSequence(scheduler)

                .BeginWith<RendererBase>()
                .Then<ParticleEmitter>();
        }

        static private SequenceController<TTask> AddSequence<TTask>(GlyphSchedulerBase<TTask> scheduler) => new SequenceController<TTask>(scheduler);

        private class SequenceController<TTask>
        {
            private readonly GlyphSchedulerBase<TTask> _scheduler;
            private readonly List<Type> _previousTypes = new List<Type>();

            public SequenceController(GlyphSchedulerBase<TTask> scheduler)
            {
                _scheduler = scheduler;
            }

            public SequenceController<TTask> BeginWith<TNext>() => BeginWith(typeof(TNext));
            public SequenceController<TTask> BeginWith(Type firstType)
            {
                _previousTypes.Clear();
                _previousTypes.Add(firstType);
                return this;
            }

            public SequenceController<TTask> Then<TNext>() => Then(typeof(TNext));
            public SequenceController<TTask> Then(Type nextType)
            {
                foreach (Type previousType in _previousTypes)
                    _scheduler.Plan(nextType).After(previousType, weight: -1);

                _previousTypes.Add(nextType);
                return this;
            }
        }
    }
}