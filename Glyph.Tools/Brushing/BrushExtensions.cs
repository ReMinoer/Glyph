using System;
using Glyph.Tools.Brushing.Decorators;

namespace Glyph.Tools.Brushing
{
    static public class BrushExtensions
    {
        static public IBrush<TCanvasOut, TArgsOut, TPaint> Retarget<TCanvasIn, TCanvasOut, TArgsIn, TArgsOut, TPaint>(this IBrush<TCanvasIn, TArgsIn, TPaint> brush, Func<TCanvasOut, TCanvasIn> targetSelector, Func<TCanvasOut, TArgsOut, TArgsIn> argsFunc)
            where TPaint : IPaint<TCanvasIn, TArgsIn>
        {
            return new RetargetedBrush<TCanvasIn, TCanvasOut, TArgsIn, TArgsOut, TPaint>(brush, targetSelector, argsFunc);
        }

        static public IBrush<TCanvas, TArgs, TPaint> Resettable<TCanvas, TArgs, TPaint>(this IBrush<TCanvas, TArgs, TPaint> brush, TPaint resetPaint, Func<TCanvas, TArgs, TPaint, bool> resetPredicate)
            where TPaint : IPaint<TCanvas, TArgs>
        {
            return new ResettableBrush<TCanvas, TArgs, TPaint>(brush, resetPaint, resetPredicate);
        }
    }
}