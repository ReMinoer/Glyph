using Glyph.Core;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming.Base
{
    public abstract class AdvancedHandleBase : HandleBase
    {
        public PrimitiveCollection DefaultPrimitives { get; } = new PrimitiveCollection();
        public PrimitiveCollection HoverPrimitives { get; } = new PrimitiveCollection();
        public PrimitiveCollection GrabbedPrimitives { get; } = new PrimitiveCollection();

        public CenteredRectangle Rectangle { get; set; }
        protected override IArea Area => new CenteredRectangle(_sceneNode.Position + Rectangle.Center, Rectangle.Size);

        public AdvancedHandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _userInterface.CursorMoved += OnCursorMoved;

            var primitiveRenderer = Add<PrimitiveRenderer>();
            primitiveRenderer.Primitives.Add(DefaultPrimitives);
            primitiveRenderer.Primitives.Add(HoverPrimitives);
            primitiveRenderer.Primitives.Add(GrabbedPrimitives);

            HoverPrimitives.Visible = false;
            GrabbedPrimitives.Visible = false;
        }

        private void OnCursorMoved(object sender, CursorEventArgs e)
        {
            HoverPrimitives.Visible = Grabbed || Area.ContainsPoint(e.CursorPosition);
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            GrabbedPrimitives.Visible = true;
        }

        protected override void OnReleased()
        {
            GrabbedPrimitives.Visible = false;
        }

        protected override void OnCancelled()
        {
            GrabbedPrimitives.Visible = false;
        }
    }
}