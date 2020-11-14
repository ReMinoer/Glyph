using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Graphics.Renderer;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming.Base
{
    public abstract class AdvancedHandleBase<TController> : HandleBase<TController>
        where TController : IAnchoredController
    {
        public VisualMeshCollection DefaultMeshes { get; } = new VisualMeshCollection();
        public VisualMeshCollection HoverMeshes { get; } = new VisualMeshCollection();
        public VisualMeshCollection GrabbedMeshes { get; } = new VisualMeshCollection();

        public CenteredRectangle Rectangle { get; set; }
        protected override IArea Area => _sceneNode.Transform(Rectangle);

        public AdvancedHandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _userInterface.CursorMoved += OnCursorMoved;

            var meshRenderer = Add<MeshRenderer>();
            meshRenderer.MeshProviders.Add(DefaultMeshes);
            meshRenderer.MeshProviders.Add(HoverMeshes);
            meshRenderer.MeshProviders.Add(GrabbedMeshes);

            HoverMeshes.Visible = false;
            GrabbedMeshes.Visible = false;
        }

        private void OnCursorMoved(object sender, CursorEventArgs e)
        {
            HoverMeshes.Visible = Grabbed || Area.ContainsPoint(e.CursorPosition);
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            GrabbedMeshes.Visible = true;
        }

        protected override void OnReleased()
        {
            GrabbedMeshes.Visible = false;
        }

        protected override void OnCancelled()
        {
            GrabbedMeshes.Visible = false;
        }
    }
}