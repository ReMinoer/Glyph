using Glyph.Animation;
using Glyph.Composition;
using Glyph.Composition.Scheduler.Base;
using Glyph.Graphics;

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
            }
        }

        public class LoadContent : SchedulerProfile<ILoadContent>
        {
            public LoadContent()
            {
                Add<SpriteTransformer>();
            }
        }

        public class Update : SchedulerProfile<IUpdate>
        {
            public Update()
            {
                Add<Motion>();
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