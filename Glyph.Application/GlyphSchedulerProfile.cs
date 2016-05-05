using Glyph.Animation;
using Glyph.Animation.Trajectories.Players;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Composition.Messaging;
using Glyph.Composition.Scheduler.Base;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Particles;
using Glyph.Physics.Colliders;
using Glyph.Scripting.Triggers;

namespace Glyph.Application
{
    public class GlyphSchedulerProfiles
    {
        public class Initialize : SchedulerProfile<IGlyphComponent>
        {
            public Initialize()
            {
                Add<SceneNode>();
                Add<ILayerRoot>();
                Add<Motion>();
                Add<ITrajectoryPlayer>();

                Add<ISpriteSource>();
                Add<SpriteTransformer>();
                Add<SpriteAnimator>();
                Add<RendererBase>();

                Add<SongPlayer>();
                Add<SoundListener>();
                Add<SoundEmitter>();

                Add<ICollider>();
                Add<TriggerArea>();
                Add<SpriteArea>();

                Add<ParticleEmitter>();
            }
        }

        public class LoadContent : SchedulerProfile<ILoadContent>
        {
            public LoadContent()
            {
                Add<SpriteLoader>();
                Add<ShapedSpriteBase>();

                Add<SongPlayer>();
            }
        }

        public class Update : SchedulerProfile<IUpdate>
        {
            public Update()
            {
                Add<Motion>();
                Add<ITrajectoryPlayer>();

                Add<SpriteAnimator>();

                Add<SongPlayer>();

                Add<ICollider>();

                Add<ParticleEmitter>();
            }
        }

        public class Draw : SchedulerProfile<IDraw>
        {
            public Draw()
            {
                Add<RendererBase>();

                Add<ParticleEmitter>();
            }
        }
    }
}