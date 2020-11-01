﻿using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.Brushing.Decorators.Base;
using Glyph.Tools.Brushing.Grid;
using Microsoft.Xna.Framework;
using Niddle;

namespace Glyph.Tools.Brushing.Decorators.Cursors
{
    public class GridCursorBrush<TBrush, TCanvas, TCell, TArgs, TPaint> : BrushDecoratorBase<TBrush, TCanvas, TArgs, TPaint, TCanvas, TArgs, TPaint>
        where TCanvas : IWriteableGrid<TCell>
        where TBrush : IBrush<TCanvas, TArgs, TPaint>
        where TArgs : IGridBrushArgs
        where TPaint : IPaint
    {
        private GridCursor _gridCursor;
        private Vector2 _cursorStartPosition;

        public bool ShowRectangle { get; }

        public GridCursorBrush(TBrush brush, bool showRectangle)
            : base(brush)
        {
            ShowRectangle = showRectangle;
        }

        public override IGlyphComponent CreateCursor(IDependencyResolver dependencyResolver)
        {
            return _gridCursor = dependencyResolver.Resolve<GridCursor>();
        }

        public override void Update(TCanvas canvas, TArgs args, TPaint paint)
        {
            base.Update(canvas, args, paint);

            _gridCursor.Rectangle = new TopLeftRectangle(canvas.ToWorldPoint(args.GridPoint), canvas.Delta);
        }

        public override void StartApply(TCanvas canvas, TArgs args, TPaint paint)
        {
            base.StartApply(canvas, args, paint);

            _cursorStartPosition = canvas.ToWorldPoint(args.GridPoint);
        }

        public override void UpdateApply(TCanvas canvas, TArgs args, TPaint paint)
        {
            base.UpdateApply(canvas, args, paint);

            if (ShowRectangle)
            {
                TopLeftRectangle rectangle = MathUtils.GetBoundingBox(_cursorStartPosition, canvas.ToWorldPoint(args.GridPoint));
                rectangle.Size += canvas.Delta;
                _gridCursor.Rectangle = rectangle;
            }
        }

        protected override TCanvas GetCanvas(TCanvas canvas) => canvas;
        protected override TArgs GetArgs(TCanvas canvas, TArgs args) => args;
        protected override TPaint GetPaint(TCanvas canvas, TArgs args, TPaint paint) => paint;
    }
}