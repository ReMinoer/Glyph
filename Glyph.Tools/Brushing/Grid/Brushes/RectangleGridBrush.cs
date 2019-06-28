using Glyph.Math;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
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

            Rectangle rectangle = MathUtils.GetBoundingBox(args.Point, _startPoint);

            for (int i = rectangle.Top; i <= rectangle.Bottom; i += 1)
                for (int j = rectangle.Left; j <= rectangle.Right; j += 1)
                {
                    var point = new Point(j, i);
                    if (!paint.CanApply(canvas, new GridBrushArgs(point)))
                        return false;
                }

            return true;
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            Rectangle rectangle = MathUtils.GetBoundingBox(args.Point, _startPoint);

            for (int i = rectangle.Top; i <= rectangle.Bottom; i += 1)
                for (int j = rectangle.Left; j <= rectangle.Right; j += 1)
                {
                    var point = new Point(j, i);
                    paint.Apply(canvas, new GridBrushArgs(point));
                }
        }
    }
}