using Glyph.Math;
using Glyph.Space;
using Glyph.Tools.Brushing.Grid.Brushes.Base;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid.Brushes
{
    public class RectangleGridBrush<TCell, TPaint> : GridBrushBase<TCell, TPaint>
        where TPaint : IGridPaint<TCell>
    {
        private Point _startPoint;

        public override void StartApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            _startPoint = args.GridPoint;
        }

        public override bool CanEndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            if (!base.CanEndApply(canvas, args, paint))
                return false;

            Rectangle rectangle = MathUtils.GetBoundingBox(args.GridPoint, _startPoint);

            for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
                for (int j = rectangle.Left; j <= rectangle.Right; j++)
                {
                    var point = new Point(j, i);
                    if (!paint.CanApply(canvas, new GridBrushArgs(point, canvas.ToWorldPoint(point))))
                        return false;
                }

            return true;
        }

        public override void EndApply(IWriteableGrid<TCell> canvas, IGridBrushArgs args, TPaint paint)
        {
            Rectangle rectangle = MathUtils.GetBoundingBox(args.GridPoint, _startPoint);

            for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
                for (int j = rectangle.Left; j <= rectangle.Right; j++)
                {
                    var point = new Point(j, i);
                    paint.Apply(canvas, new GridBrushArgs(point, canvas.ToWorldPoint(point)));
                }
        }
    }
}