using System.Linq;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Tools.Brushing.Controllers.Base;
using Glyph.Tools.Brushing.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Controllers
{
    public abstract class SimpleCursorBrushControllerBase<TCanvas, TBrush, TPaint> : CursorBrushControllerBase<TCanvas, TBrush, ISpaceBrushArgs, TPaint>
        where TBrush : IBrush<TCanvas, ISpaceBrushArgs, TPaint>
        where TPaint : IPaint
    {
        public SimpleCursorBrushControllerBase(GlyphResolveContext glyphResolveContext, RootView rootView, ProjectionManager projectionManager)
            : base(glyphResolveContext, rootView, projectionManager)
        {
        }
        
        protected override sealed ISpaceBrushArgs GetBrushArgs(TCanvas canvas, Vector2 rootViewCursorPosition)
        {
            ProjectionManager.IOptionsController<Vector2> controller = ProjectionManager.ProjectFromPosition(RootView, rootViewCursorPosition).To(GetCanvasTargetSceneNode(canvas));
            if (RaycastClient != null)
                controller = controller.ByRaycast().ForDrawClient(RaycastClient);

            return new SpaceBrushArgs(controller.First().Value);
        }

        protected abstract ISceneNode GetCanvasTargetSceneNode(TCanvas canvas);
    }

    public class EngineCursorBrushController : SimpleCursorBrushControllerBase<IGlyphComponent, IBrush<IGlyphComponent, ISpaceBrushArgs, IPaint>, IPaint>
    {
        public EngineCursorBrushController(GlyphResolveContext glyphResolveContext, RootView rootView, ProjectionManager projectionManager)
            : base(glyphResolveContext, rootView, projectionManager)
        {
        }

        protected override ISceneNode GetCanvasTargetSceneNode(IGlyphComponent canvas) => canvas.GetSceneNode();
    }

    public class DataCursorBrushController : SimpleCursorBrushControllerBase<IGlyphData, IBrush<IGlyphData, ISpaceBrushArgs, IPaint>, IPaint>
    {
        public DataCursorBrushController(GlyphResolveContext glyphResolveContext, RootView rootView, ProjectionManager projectionManager)
            : base(glyphResolveContext, rootView, projectionManager)
        {
        }

        protected override ISceneNode GetCanvasTargetSceneNode(IGlyphData canvas) => canvas.BindedObject.GetSceneNode();
    }
}