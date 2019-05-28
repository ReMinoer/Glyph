using System;
using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Space;
using Glyph.UI;
using Microsoft.Xna.Framework;
using IControl = Fingear.IControl;

namespace Glyph.Tools
{
    public interface IBrush
    {
    }

    public interface IBrush<in TCanvas, in TArgs, in TPaint> : IBrush
        where TPaint : IPaint<TCanvas, TArgs>
    {
        bool CanStartApply(TCanvas canvas, TArgs args, TPaint paint);
        void StartApply(TCanvas canvas, TArgs args, TPaint paint);
        void UpdateApply(TCanvas canvas, TArgs args, TPaint paint);
        bool CanEndApply(TCanvas canvas, TArgs args, TPaint paint);
        void EndApply(TCanvas canvas, TArgs args, TPaint paint);
        void OnInvalidStart(TCanvas canvas, TArgs args, TPaint paint);
        void OnCancellation(TCanvas canvas, TArgs args, TPaint paint);
        void OnInvalidEnd(TCanvas canvas, TArgs args, TPaint paint);
    }

    public interface IPaint<in TCanvas, in TArgs>
    {
        bool CanApply(TCanvas canvas, TArgs args);
        void Apply(TCanvas canvas, TArgs args);
    }

    public interface IGridBrushArgs
    {
        Point Point { get; }
    }

    public class GridBrushArgs : IGridBrushArgs
    {
        public Point Point { get; set; }
        public GridBrushArgs() {}
        public GridBrushArgs(Point point) => Point = point;
    }

    public interface IGridBrush<TCell, in TPaint> : IBrush<IWriteableGrid<TCell>, IGridBrushArgs, TPaint>
        where TPaint : IPaint<IWriteableGrid<TCell>, IGridBrushArgs>
    {
    }

    public interface IGridPaint<TCell> : IPaint<IWriteableGrid<TCell>, IGridBrushArgs>
    {
    }

    public abstract class BrushBase<TCanvas, TBrushArgs, TPaint> : IBrush<TCanvas, TBrushArgs, TPaint>
        where TPaint : IPaint<TCanvas, TBrushArgs>
    {
        public virtual bool CanStartApply(TCanvas canvas, TBrushArgs args, TPaint paint) => true;
        public virtual void StartApply(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void UpdateApply(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual bool CanEndApply(TCanvas canvas, TBrushArgs args, TPaint paint) => true;
        public virtual void EndApply(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void OnInvalidStart(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void OnCancellation(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void OnInvalidEnd(TCanvas canvas, TBrushArgs args, TPaint paint) { }
    }

    public abstract class GridBrushBase<TCell, TPaint> : BrushBase<IWriteableGrid<TCell>, IGridBrushArgs, TPaint>, IGridBrush<TCell, TPaint>
        where TPaint : IPaint<IWriteableGrid<TCell>, IGridBrushArgs>
    {
        public override bool CanStartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => canvas.ContainsPoint(args.Point);
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => canvas.ContainsPoint(args.Point);
    }

    public class DraggableGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            return base.CanEndApply(canvas, args, paint) && paint.CanApply(canvas, args);
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            paint.Apply(canvas, args);
        }
    }

    public class ContinuousGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private Point _currentPoint;

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            _currentPoint = args.Point;
        }

        public override void UpdateApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            if (_currentPoint == args.Point)
                return;

            if (!paint.CanApply(canvas, args))
                return;

            paint.Apply(canvas, args);
            _currentPoint = args.Point;
        }
        
        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint) => true;
    }

    public class RectangleGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private Point _startPoint;

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            _startPoint = args.Point;
        }

        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            if (!base.CanEndApply(canvas, args, paint))
                return false;
            
            int iterX = System.Math.Sign(args.Point.X - _startPoint.X);
            int iterY = System.Math.Sign(args.Point.Y - _startPoint.Y);

            for (int i = 0; i < _startPoint.Y; i += iterY)
                for (int j = 0; j < _startPoint.X; j += iterX)
                {
                    var point = new Point(_startPoint.X + j, _startPoint.Y + i);
                    if (!paint.CanApply(canvas, new GridBrushArgs(point)))
                        return false;
                }

            return true;
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            int iterX = System.Math.Sign(args.Point.X - _startPoint.X);
            int iterY = System.Math.Sign(args.Point.Y - _startPoint.Y);

            for (int i = 0; i < _startPoint.Y; i += iterY)
                for (int j = 0; j < _startPoint.X; j += iterX)
                {
                    var point = new Point(_startPoint.X + j, _startPoint.Y + i);
                    paint.Apply(canvas, new GridBrushArgs(point));
                }
        }
    }

    public class CellValuePaint<TCell> : IGridPaint<TCell>
    {
        public TCell Value { get; set; }

        public virtual bool CanApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args) => true;

        public void Apply(IWriteableGrid<TCell> canvas, IGridBrushArgs args)
        {
            canvas[args.Point] = Value;
        }
    }

    public interface IIntegratedEditor : IGlyphComponent, ILoadContent, IUpdate, IDraw
    {
        object EditedObject { get; }
    }

    public interface IIntegratedEditor<out T> : IIntegratedEditor
    {
        new T EditedObject { get; }
    }

    public abstract class BrushControllerBase<TCanvas, TBrushArgs, TPaint> : GlyphObject, IIntegratedEditor<TCanvas>
        where TPaint : IPaint<TCanvas, TBrushArgs>
    {
        public TCanvas Canvas { get; set; }
        public IBrush<TCanvas, TBrushArgs, TPaint> Brush { get; set; }
        public TPaint Paint { get; set; }
        public bool ApplyingBrush { get; private set; }

        object IIntegratedEditor.EditedObject => Canvas;
        TCanvas IIntegratedEditor<TCanvas>.EditedObject => Canvas;

        public event EventHandler ApplyStarted;
        public event EventHandler ApplyCancelled;
        public event EventHandler ApplyEnded;

        public BrushControllerBase(GlyphResolveContext context)
            : base(context)
        {
            Schedulers.Update.Plan(UpdateLocal);
        }

        private void UpdateLocal(ElapsedTime elapsedtime)
        {
            TBrushArgs args = GetBrushArgs();

            if (!ApplyingBrush)
            {
                if (RequestApplyStart())
                {
                    if (Brush.CanStartApply(Canvas, args, Paint))
                    {
                        ApplyingBrush = true;
                        Brush.StartApply(Canvas, args, Paint);
                        ApplyStarted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Brush.OnInvalidStart(Canvas, args, Paint);
                    }
                }
            }
            else
            {
                if (RequestApplyEnd())
                {
                    ApplyingBrush = false;

                    if (Brush.CanEndApply(Canvas, args, Paint))
                    {
                        Brush.EndApply(Canvas, args, Paint);
                        ApplyEnded?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Brush.OnInvalidEnd(Canvas, args, Paint);
                    }
                }
            }

            if (ApplyingBrush)
            {
                if (RequestCancellation())
                {
                    ApplyingBrush = false;
                    Brush.OnCancellation(Canvas, args, Paint);
                    ApplyCancelled?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Brush.UpdateApply(Canvas, args, Paint);
                }
            }
        }

        protected abstract bool RequestApplyStart();
        protected abstract bool RequestApplyEnd();
        protected abstract bool RequestCancellation();
        protected abstract TBrushArgs GetBrushArgs();
    }

    public abstract class CursorBrushControllerBase<TCanvas, TBrushArgs, TPaint> : BrushControllerBase<TCanvas, TBrushArgs, TPaint>
        where TPaint : IPaint<TCanvas, TBrushArgs>
    {
        private readonly ProjectionCursorControl _cursor;
        private readonly Control _applyBrush;
        private readonly IControl _cancel;

        protected readonly SceneNode SceneNode;

        public IInput Input
        {
            get => _applyBrush.Input;
            set => _applyBrush.Input = value;
        }

        protected CursorBrushControllerBase(GlyphResolveContext context, RootView rootView, ProjectionManager projectionManager)
            : base(context)
        {
            SceneNode = Add<SceneNode>();

            Add<Controls>().AddMany(new []
            {
                _cursor = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, rootView, new ReadOnlySceneNodeDelegate(() => SceneNode), projectionManager),
                _applyBrush = new Control {DesiredActivity = InputActivity.Pressed},
                _cancel = MenuControls.Instance.Cancel
            });

            Schedulers.Update.Plan(UpdateBrushPosition).AtStart();
        }

        private void UpdateBrushPosition(ElapsedTime elapsedtime)
        {
            if (_cursor.IsActive(out System.Numerics.Vector2 cursorPosition))
                SceneNode.Position = cursorPosition.AsMonoGameVector();
        }

        protected override bool RequestApplyStart() => _applyBrush.IsActive;
        protected override bool RequestApplyEnd() => !_applyBrush.IsActive;
        protected override bool RequestCancellation() => _cancel.IsActive;

        protected override TBrushArgs GetBrushArgs() => GetBrushArgs(SceneNode.Position);
        protected abstract TBrushArgs GetBrushArgs(Vector2 position);
    }

    public class GridBrushController<TCell, TPaint> : CursorBrushControllerBase<IWriteableGrid<TCell>, IGridBrushArgs, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        public GridBrushController(GlyphResolveContext glyphResolveContext, RootView rootView, ProjectionManager projectionManager)
            : base(glyphResolveContext, rootView, projectionManager)
        {
        }

        protected override IGridBrushArgs GetBrushArgs(Vector2 position)
        {
            return new GridBrushArgs { Point = Canvas.ToGridPoint(position) };
        }
    }

    public class GridEditor<TCell, TPaint> : GlyphObject, IIntegratedEditor<IWriteableGrid<TCell>>
        where TPaint : IGridPaint<TCell>
    {
        private IWriteableGrid<TCell> _grid;
        private readonly GridBrushController<TCell, TPaint>[] _brushControllers;
        private readonly GridBrushController<TCell, TPaint> _leftBrushController;
        private readonly GridBrushController<TCell, TPaint> _rightBrushController;

        public IWriteableGrid<TCell> Grid
        {
            get => _grid;
            set
            {
                _grid = value;

                foreach (GridBrushController<TCell, TPaint> brushController in _brushControllers)
                    brushController.Canvas = _grid;
            }
        }

        public TPaint LeftClicPaint
        {
            get => _leftBrushController.Paint;
            set => _leftBrushController.Paint = value;
        }

        public TPaint RightClicPaint
        {
            get => _rightBrushController.Paint;
            set => _rightBrushController.Paint = value;
        }

        object IIntegratedEditor.EditedObject => Grid;
        IWriteableGrid<TCell> IIntegratedEditor<IWriteableGrid<TCell>>.EditedObject => Grid;

        public GridEditor(GlyphResolveContext context)
            : base(context)
        {
            _brushControllers = new []
            {
                _leftBrushController = Add<GridBrushController<TCell, TPaint>>(),
                _rightBrushController = Add<GridBrushController<TCell, TPaint>>()
            };

            foreach (GridBrushController<TCell, TPaint> brushController in _brushControllers)
            {
                brushController.ApplyStarted += BrushControllerOnApplyStarted;
                brushController.ApplyCancelled += BrushControllerOnApplyEnded;
                brushController.ApplyEnded += BrushControllerOnApplyEnded;
            }
        }

        private void BrushControllerOnApplyStarted(object sender, EventArgs e)
        {
            foreach (GridBrushController<TCell, TPaint> brushController in _brushControllers.Where(x => x != sender))
                brushController.Enabled = false;
        }

        private void BrushControllerOnApplyEnded(object sender, EventArgs e)
        {
            foreach (GridBrushController<TCell, TPaint> brushController in _brushControllers)
                brushController.Enabled = true;
        }
    }
}