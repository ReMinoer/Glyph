using System;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Scheduling;

namespace Glyph.Graphics.Renderer.Base
{
    public abstract class RendererBase : GlyphComponent, IDraw, IBoxedComponent
    {
        protected virtual float RenderDepthOverride => SceneNode.Depth;
        float IDrawTask.RenderDepth => RenderDepthOverride;

        protected abstract ISceneNode SceneNode { get; }
        ISceneNode IDrawTask.SceneNode => SceneNode;

        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public abstract IArea Area { get; }

        public abstract void Draw(IDrawer drawer);

        public event EventHandler RenderDepthChanged;
        private ISceneNode _depthSubscription;

        protected void SubscribeDepthChanged(ISceneNode sceneNode)
        {
            UnsubscribeDepthChanged();

            sceneNode.DepthChanged += OnSceneNodeDepthChanged;
            _depthSubscription = sceneNode;
        }

        private void UnsubscribeDepthChanged()
        {
            if (_depthSubscription == null)
                return;

            _depthSubscription.DepthChanged -= OnSceneNodeDepthChanged;
            _depthSubscription = null;
        }

        private void OnSceneNodeDepthChanged(object sender, EventArgs e) => RenderDepthChanged?.Invoke(this, EventArgs.Empty);

        public override void Dispose()
        {
            UnsubscribeDepthChanged();
            base.Dispose();
        }
    }
}