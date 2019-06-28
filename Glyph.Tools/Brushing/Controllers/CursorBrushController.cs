using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Tools.Brushing.Controllers.Base;
using Glyph.Tools.Brushing.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Controllers
{
    public class CursorBrushController<TCanvas, TPaint> : CursorBrushControllerBase<TCanvas, ISpaceBrushArgs, TPaint>
        where TCanvas : IGlyphComponent
        where TPaint : IPaint
    {
        public CursorBrushController(GlyphResolveContext glyphResolveContext, RootView rootView, ProjectionManager projectionManager)
            : base(glyphResolveContext, rootView, projectionManager)
        {
        }
        
        protected override ISpaceBrushArgs GetBrushArgs(TCanvas canvas, Vector2 rootViewCursorPosition)
        {
            ProjectionManager.IOptionsController<Vector2> controller = ProjectionManager.ProjectFromPosition(RootView, rootViewCursorPosition).To(canvas.GetSceneNode());
            if (RaycastClient != null)
                controller = controller.ByRaycast().ForDrawClient(RaycastClient);

            return new SpaceBrushArgs(controller.First().Value);
        }
    }
}