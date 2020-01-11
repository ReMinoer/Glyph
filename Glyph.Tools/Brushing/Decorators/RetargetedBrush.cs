using System;

namespace Glyph.Tools.Brushing.Decorators
{
    public class RetargetedBrush<TCanvasIn, TCanvasOut, TArgsIn, TArgsOut, TPaint> : IBrush<TCanvasOut, TArgsOut, TPaint>
        where TPaint : IPaint<TCanvasIn, TArgsIn>
    {
        private readonly IBrush<TCanvasIn, TArgsIn, TPaint> _brush;
        private readonly Func<TCanvasOut, TCanvasIn> _targetSelector;
        private readonly Func<TCanvasOut, TArgsOut, TArgsIn> _argsFunc;

        public RetargetedBrush(IBrush<TCanvasIn, TArgsIn, TPaint> brush, Func<TCanvasOut, TCanvasIn> targetSelector, Func<TCanvasOut, TArgsOut, TArgsIn> argsFunc)
        {
            _brush = brush;
            _targetSelector = targetSelector;
            _argsFunc = argsFunc;
        }

        public bool CanStartApply(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.CanStartApply(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public void StartApply(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.StartApply(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public void UpdateApply(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.UpdateApply(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public bool CanEndApply(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.CanEndApply(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public void EndApply(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.EndApply(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public void OnInvalidStart(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.OnInvalidStart(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public void OnCancellation(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.OnCancellation(_targetSelector(canvas), _argsFunc(canvas, args), paint);
        public void OnInvalidEnd(TCanvasOut canvas, TArgsOut args, TPaint paint) => _brush.OnInvalidEnd(_targetSelector(canvas), _argsFunc(canvas, args), paint);
    }
}