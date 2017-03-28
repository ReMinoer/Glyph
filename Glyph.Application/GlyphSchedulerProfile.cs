using Glyph.Animation;
using Glyph.Animation.Trajectories.Players;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Scheduler.Base;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Particles;
using Glyph.Scripting;

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

                Add<ParticleEmitter>();

                Add<ICollider>();
                Add<SpriteArea>();

                Add<Trigger>();
                Add<Actor>();
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

                Add<Actor>();
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