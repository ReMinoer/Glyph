using System.Linq;
using Diese.Collections;
using Fingear.Controls;
using Fingear.Inputs;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.UI;
using Microsoft.Xna.Framework;
using IControl = Fingear.Controls.IControl;

namespace Glyph.Tools.Brushing.Controllers.Base
{
    public abstract class CursorBrushControllerBase<TCanvas, TBrush, TBrushArgs, TPaint> : BrushControllerBase<TCanvas, TBrush, TBrushArgs, TPaint>
        where TBrush : IBrush<TCanvas, TBrushArgs, TPaint>
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

        private IGlyphComponent _cursorComponent;
        public override TBrush Brush
        {
            get => base.Brush;
            set
            {
                base.Brush = value;

                IGlyphComponent newCursor = Brush?.CreateCursor(Resolver);
                SetPropertyComponent(ref _cursorComponent, newCursor, disposeOnRemove: true);
            }
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

        private void UpdateBrushPosition(ElapsedTime elapsedTime)
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