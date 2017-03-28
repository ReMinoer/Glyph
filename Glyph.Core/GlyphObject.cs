using System;
using System.Linq;
using Diese.Collections;
using Diese.Injection;
using Glyph.Composition;
using Glyph.Composition.Delegates;
using Glyph.Core.Scheduler;
using Glyph.Math;
using Glyph.Math.Shapes;
using Stave;

namespace Glyph.Core
{
    // TASK : get Injectable at runtime
    public class GlyphObject : GlyphSchedulableBase, IBoxedComponent
    {
        protected readonly SchedulerHandler Schedulers;

        public GlyphObject(IDependencyInjector injector)
            : this(injector, new SchedulerHandler(injector))
        {
        }

        private GlyphObject(IDependencyInjector injector, SchedulerHandler schedulerHandler)
            : base(injector, schedulerHandler)
        {
            Schedulers = schedulerHandler;
        }

        public override sealed void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            foreach (DrawDelegate draw in Schedulers.Draw.Planning)
                draw(drawer);
        }

        ISceneNode IBoxedComponent.SceneNode => Components.FirstOrDefault<ISceneNode>() ?? this.ParentQueue().SelectMany(x => x.Components).First<ISceneNode>();
        IArea IBoxedComponent.Area => MathUtils.GetBoundingBox(Components.OfType<IBoxedComponent>().Select(x => x.Area));

        protected class SchedulerHandler : SchedulerHandlerBase
        {
            public GlyphScheduler<IDraw, DrawDelegate> Draw { get; private set; }

            public SchedulerHandler(IDependencyInjector injector)
                : base(injector)
            {
                Draw = AddScheduler<IDraw, DrawDelegate>();
            }
        }
    }
}