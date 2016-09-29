using Diese.Injection;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;

namespace Glyph.Game
{
    public abstract class SceneBase<TSchedulerHandler> : GlyphSchedulableBase, IScene
        where TSchedulerHandler : GlyphSchedulableBase.SchedulerHandlerBase
    {
        protected TSchedulerHandler Schedulers { get; }
        protected abstract ISceneRenderer Renderer { get; }
        public abstract SceneNode RootNode { get; }

        protected SceneBase(IDependencyInjector injector, TSchedulerHandler schedulerHandler)
            : base(injector, schedulerHandler)
        {
            Schedulers = schedulerHandler;
        }

        public override sealed void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            Renderer.RenderingProcess(drawer);
        }
    }
}