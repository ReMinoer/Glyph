using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.UI;
using Microsoft.Xna.Framework;
using IControl = Fingear.IControl;

namespace Glyph.Tools.Brushing.Controllers.Base
{
    public abstract class CursorBrushControllerBase<TCanvas, TBrushArgs, TPaint> : BrushControllerBase<TCanvas, TBrushArgs, TPaint>
        where TPaint : IPaint
    {
        protected readonly RootView RootView;
        protected readonly ProjectionManager ProjectionManager;

        private readonly SceneNode _sceneNode;
        private readonly ProjectionCursorControl _cursor;
        private readonly Control _applyBrush;
        private readonly IControl _cancel;
        
        public IDrawClient RaycastClient { get; set; }
        public IInput Input
        {
            get => _applyBrush.Input;
            set => _applyBrush.Input = value;
        }

        protected CursorBrushControllerBase(GlyphResolveContext context, RootView rootView, ProjectionManager projectionManager)
            : base(context)
        {
            RootView = rootView;
            ProjectionManager = projectionManager;
            _sceneNode = Add<SceneNode>();

            Add<Controls>().AddMany(new []
            {
                _cursor = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, rootView, rootView, projectionManager),
                _applyBrush = new Control {DesiredActivity = InputActivity.Pressed},
                _cancel = UserInterfaceControls.Instance.Cancel
            });

            Schedulers.Update.Plan(UpdateBrushPosition).AtStart();
        }

        private void UpdateBrushPosition(ElapsedTime elapsedtime)
        {
            if (_cursor.IsActive(out System.Numerics.Vector2 cursorPosition))
            {
                ProjectionManager.IOptionsController<Vector2> controller = ProjectionManager.ProjectFromPosition(RootView, cursorPosition.AsMonoGameVector()).To(_sceneNode);
                if (RaycastClient != null)
                    controller = controller.ByRaycast().ForDrawClient(RaycastClient);
                
                _sceneNode.Position = controller.First().Value;
            }
        }

        protected override bool RequestingApplyStart => _applyBrush.IsActive;
        protected override bool RequestingApplyEnd => !_applyBrush.IsActive;
        protected override bool RequestingCancellation => _cancel.IsActive;
        
        protected override sealed TBrushArgs GetBrushArgs(TCanvas canvas)
        {
            _cursor.IsActive(out System.Numerics.Vector2 cursorPosition);
            return GetBrushArgs(canvas, cursorPosition.AsMonoGameVector());
        }

        protected abstract TBrushArgs GetBrushArgs(TCanvas canvas, Vector2 rootViewCursorPosition);
    }
}