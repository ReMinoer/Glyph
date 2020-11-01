using System;
using Glyph.Space;
using Glyph.Tools.Brushing.Decorators;
using Glyph.Tools.Brushing.Decorators.Cursors;
using Glyph.Tools.Brushing.Grid;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing
{
    static public class BrushExtensions
    {
        static public RetargetedBrush<TBrush, TBrushCanvas, TBrushArgs, TCanvas, TArgs, TPaint> Retarget<TBrush, TBrushCanvas, TBrushArgs, TCanvas, TArgs, TPaint>(
            this TBrush brush, Func<TCanvas, TBrushCanvas> targetSelector, Func<TCanvas, TArgs, TBrushArgs> argsFunc)
            where TBrush : IBrush<TBrushCanvas, TBrushArgs, TPaint>
            where TPaint : IPaint
        {
            return new RetargetedBrush<TBrush, TBrushCanvas, TBrushArgs, TCanvas, TArgs, TPaint>(brush, targetSelector, argsFunc);
        }

        static public IBrush<TCanvas, TArgs, TPaint> Resettable<TCanvas, TArgs, TPaint>(
            this IBrush<TCanvas, TArgs, TPaint> brush, TPaint resetPaint, Func<TCanvas, TArgs, TPaint, bool> resetPredicate)
            where TPaint : IPaint<TCanvas, TArgs>
        {
            return new ResettableBrush<IBrush<TCanvas, TArgs, TPaint>, TCanvas, TArgs, TPaint>(brush, resetPaint, resetPredicate);
        }

        static public IBrush<IWriteableGrid<TCell>, TArgs, TPaint> GridCursor<TCell, TArgs, TPaint>(
            this IBrush<IWriteableGrid<TCell>, TArgs, TPaint> brush, Point? size = null, bool showRectangle = false)
            where TArgs : IGridBrushArgs
            where TPaint : IPaint<IWriteableGrid<TCell>, TArgs>
        {
            return new GridCursorBrush<IBrush<IWriteableGrid<TCell>, TArgs, TPaint>, IWriteableGrid<TCell>, TCell, TArgs, TPaint>(brush, size, showRectangle);
        }
    }
}