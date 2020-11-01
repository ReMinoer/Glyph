using System;
using Glyph.Tools.Brushing.Decorators.Base;

namespace Glyph.Tools.Brushing.Decorators
{
    public class ResettableBrush<TBrush, TCanvas, TArgs, TPaint> : BrushDecoratorBase<TBrush, TCanvas, TArgs, TPaint, TCanvas, TArgs, TPaint>
        where TBrush : IBrush<TCanvas, TArgs, TPaint>
        where TPaint : IPaint
    {
        private readonly TPaint _resetPaint;
        private readonly Func<TCanvas, TArgs, TPaint, bool> _resetPredicate;

        private TPaint _currentPaint;

        public ResettableBrush(TBrush brush, TPaint resetPaint, Func<TCanvas, TArgs, TPaint, bool> resetPredicate)
            : base(brush)
        {
            _resetPaint = resetPaint;
            _resetPredicate = resetPredicate;
        }

        public override void StartApply(TCanvas canvas, TArgs args, TPaint paint)
        {
            _currentPaint = _resetPredicate(canvas, args, paint) ? _resetPaint : paint;
            base.StartApply(canvas, args, paint);
        }

        protected override TCanvas GetCanvas(TCanvas canvas) => canvas;
        protected override TArgs GetArgs(TCanvas canvas, TArgs args) => args;
        protected override TPaint GetPaint(TCanvas canvas, TArgs args, TPaint paint) => _currentPaint;
    }
}