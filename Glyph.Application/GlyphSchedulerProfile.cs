using Glyph.Animation;
using Glyph.Animation.Trajectories.Players;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Composition.Scheduler.Base;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Physics.Colliders;

namespace Glyph.Application
{
    public class GlyphSchedulerProfiles
    {
        public class Initialize : SchedulerProfile<IGlyphComponent>
        {
            public Initialize()
            {
                Add<SceneNode>();
                Add<Motion>();
                Add<ITrajectoryPlayer>();

                Add<ISpriteSource>();
                Add<SpriteTransformer>();
                Add<SpriteAnimator>();
                Add<SpriteRenderer>();

                Add<SongPlayer>();
                Add<SoundListener>();
                Add<SoundEmitter>();

                Add<ICollider>();
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
            }
        }

        public class Draw : SchedulerProfile<IDraw>
        {
            public Draw()
            {
                Add<SpriteRenderer>();
            }
        }
    }
}