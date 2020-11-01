using System;
using Glyph.Tools.Brushing.Decorators.Base;

namespace Glyph.Tools.Brushing.Decorators
{
    public class RetargetedBrush<TBrush, TBrushCanvas, TBrushArgs, TCanvas, TArgs, TPaint> : BrushDecoratorBase<TBrush, TBrushCanvas, TBrushArgs, TPaint, TCanvas, TArgs, TPaint>
        where TBrush : IBrush<TBrushCanvas, TBrushArgs, TPaint>
        where TPaint : IPaint
    {
        private readonly Func<TCanvas, TBrushCanvas> _targetSelector;
        private readonly Func<TCanvas, TArgs, TBrushArgs> _argsFunc;

        public RetargetedBrush(TBrush brush, Func<TCanvas, TBrushCanvas> targetSelector, Func<TCanvas, TArgs, TBrushArgs> argsFunc)
            : base(brush)
        {
            _targetSelector = targetSelector;
            _argsFunc = argsFunc;
        }

        protected override TBrushCanvas GetCanvas(TCanvas canvas) => _targetSelector(canvas);
        protected override TBrushArgs GetArgs(TCanvas canvas, TArgs args) => _argsFunc(canvas, args);
        protected override TPaint GetPaint(TCanvas canvas, TArgs args, TPaint paint) => paint;
    }
}