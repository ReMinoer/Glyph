﻿using Diese.Injection;
using Glyph.Composition;
using Glyph.Graphics;

namespace Glyph.Game
{
    public abstract class SceneBase<TSchedulerHandler> : GlyphSchedulableBase, IScene
        where TSchedulerHandler : GlyphSchedulableBase.SchedulerHandlerBase
    {
        public abstract SceneNode RootNode { get; }

        protected override sealed SchedulerHandlerBase SchedulerAssigner
        {
            get { return Schedulers; }
        }

        protected abstract TSchedulerHandler Schedulers { get; }
        protected abstract ISceneRenderer Renderer { get; }

        protected SceneBase(IDependencyInjector injector)
            : base(injector)
        {
        }

        public override sealed void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            Renderer.RenderingProcess(drawer);
        }
    }
}