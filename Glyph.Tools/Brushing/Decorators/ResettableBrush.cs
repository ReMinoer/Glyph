using System;

namespace Glyph.Tools.Brushing.Decorators
{
    public class ResettableBrush<TCanvas, TArgs, TPaint> : IBrush<TCanvas, TArgs, TPaint>
        where TPaint : IPaint
    {
        private readonly IBrush<TCanvas, TArgs, TPaint> _brush;
        private readonly TPaint _resetPaint;
        private readonly Func<TCanvas, TArgs, TPaint, bool> _resetPredicate;

        private TPaint _currentPaint;

        public ResettableBrush(IBrush<TCanvas, TArgs, TPaint> brush, TPaint resetPaint, Func<TCanvas, TArgs, TPaint, bool> resetPredicate)
        {
            _brush = brush;
            _resetPaint = resetPaint;
            _resetPredicate = resetPredicate;
        }

        private TPaint GetPaint(TCanvas canvas, TArgs args, TPaint paint)
        {
            return _resetPredicate(canvas, args, paint) ? _resetPaint : paint;
        }

        public bool CanStartApply(TCanvas canvas, TArgs args, TPaint paint) => _brush.CanStartApply(canvas, args, GetPaint(canvas, args, GetPaint(canvas, args, paint)));
        public void StartApply(TCanvas canvas, TArgs args, TPaint paint)
        {
            _currentPaint = GetPaint(canvas, args, paint);
            _brush.StartApply(canvas, args, GetPaint(canvas, args, _currentPaint));
        }

        public void UpdateApply(TCanvas canvas, TArgs args, TPaint paint) => _brush.UpdateApply(canvas, args, _currentPaint);
        public bool CanEndApply(TCanvas canvas, TArgs args, TPaint paint) => _brush.CanEndApply(canvas, args, _currentPaint);
        public void EndApply(TCanvas canvas, TArgs args, TPaint paint) => _brush.EndApply(canvas, args, _currentPaint);
        public void OnInvalidStart(TCanvas canvas, TArgs args, TPaint paint) => _brush.OnInvalidStart(canvas, args, _currentPaint);
        public void OnCancellation(TCanvas canvas, TArgs args, TPaint paint) => _brush.OnCancellation(canvas, args, _currentPaint);
        public void OnInvalidEnd(TCanvas canvas, TArgs args, TPaint paint) => _brush.OnInvalidEnd(canvas, args, _currentPaint);
    }
}