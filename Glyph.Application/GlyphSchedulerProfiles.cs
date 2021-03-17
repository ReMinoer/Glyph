using System;
using System.Collections.Generic;
using Glyph.Animation;
using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories.Players;
using Glyph.Audio;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Layers;
using Glyph.Graphics;
using Glyph.Graphics.Renderer.Base;
using Glyph.Graphics.Shapes;
using Glyph.Particles;
using Glyph.Scheduling;
using Glyph.Scheduling.Base;
using Glyph.Scripting;

namespace Glyph.Application
{
    static public class DefaultGlyphSchedulerRules
    {
        static public InitializeScheduler Setup(InitializeScheduler scheduler)
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

            return scheduler;
        }

        static public LoadContentScheduler Setup(LoadContentScheduler scheduler)
        {
            AddSequence(scheduler)

                .BeginWith<SpriteLoader>()
                .Then<ShapedSpriteBase>()
                .Then<SongPlayer>();

            return scheduler;
        }

        static public UpdateScheduler Setup(UpdateScheduler scheduler)
        {
            AddSequence(scheduler)

                .BeginWith<PositionBinding>()
                .Then<ITrajectoryPlayer>()
                .Then<MotorBase>()
                .Then<Motion>()

                .Then<SpriteAnimator>()
                .Then<SongPlayer>()
                .Then<ICollider>()
                .Then<Actor>();

            return scheduler;
        }

        static private SequenceController<TTask, TDelegate> AddSequence<TTask, TDelegate>(GlyphSchedulerBase<TTask, TDelegate> scheduler)
            where TTask : class
            => new SequenceController<TTask, TDelegate>(scheduler);

        private class SequenceController<TTask, TDelegate>
            where TTask : class
        {
            private readonly GlyphSchedulerBase<TTask, TDelegate> _scheduler;
            private readonly List<Type> _previousTypes = new List<Type>();

            public SequenceController(GlyphSchedulerBase<TTask, TDelegate> scheduler)
            {
                _scheduler = scheduler;
            }

            public SequenceController<TTask, TDelegate> BeginWith<TNext>() where TNext : TTask => BeginWith(typeof(TNext));
            public SequenceController<TTask, TDelegate> BeginWith(Type firstType)
            {
                _previousTypes.Clear();
                _previousTypes.Add(firstType);
                return this;
            }

            public SequenceController<TTask, TDelegate> Then<TNext>() where TNext : TTask => Then(typeof(TNext));
            public SequenceController<TTask, TDelegate> Then(Type nextType)
            {
                foreach (Type previousType in _previousTypes)
                    _scheduler.Plan(nextType).After(previousType).WithWeight(-1);

                _previousTypes.Add(nextType);
                return this;
            }
        }
    }
}