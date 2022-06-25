using Glyph.Composition;
using Glyph.Tools.UndoRedo;
using Niddle;

namespace Glyph.Tools.Brushing.Decorators.Base
{
    public abstract class BrushDecoratorBase<TBrush, TBrushCanvas, TBrushArgs, TBrushPaint, TCanvas, TArgs, TPaint> : IBrush<TCanvas, TArgs, TPaint>
        where TBrush : IBrush<TBrushCanvas, TBrushArgs, TBrushPaint>
        where TBrushPaint : IPaint
        where TPaint : IPaint
    {
        protected readonly TBrush Brush;

        protected BrushDecoratorBase(TBrush brush)
        {
            Brush = brush;
        }

        public virtual IGlyphComponent CreateCursor(IDependencyResolver dependencyResolver) => Brush.CreateCursor(dependencyResolver);
        public virtual void Update(TCanvas canvas, TArgs args, TPaint paint) => Brush.Update(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual bool CanStartApply(TCanvas canvas, TArgs args, TPaint paint) => Brush.CanStartApply(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual void StartApply(TCanvas canvas, TArgs args, TPaint paint) => Brush.StartApply(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual void UpdateApply(TCanvas canvas, TArgs args, TPaint paint) => Brush.UpdateApply(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual bool CanEndApply(TCanvas canvas, TArgs args, TPaint paint) => Brush.CanEndApply(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual void EndApply(TCanvas canvas, TArgs args, TPaint paint, IUndoRedoStack undoRedoStack) => Brush.EndApply(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint), undoRedoStack);
        public virtual void OnInvalidStart(TCanvas canvas, TArgs args, TPaint paint) => Brush.OnInvalidStart(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual void OnCancellation(TCanvas canvas, TArgs args, TPaint paint) => Brush.OnCancellation(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));
        public virtual void OnInvalidEnd(TCanvas canvas, TArgs args, TPaint paint) => Brush.OnInvalidEnd(GetCanvas(canvas), GetArgs(canvas, args), GetPaint(canvas, args, paint));

        protected abstract TBrushCanvas GetCanvas(TCanvas canvas);
        protected abstract TBrushArgs GetArgs(TCanvas canvas, TArgs args);
        protected abstract TBrushPaint GetPaint(TCanvas canvas, TArgs args, TPaint paint);
    }
}