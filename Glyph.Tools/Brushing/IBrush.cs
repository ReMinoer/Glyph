using Glyph.Composition;
using Glyph.Tools.UndoRedo;
using Niddle;

namespace Glyph.Tools.Brushing
{
    public interface IBrush
    {
        IGlyphComponent CreateCursor(IDependencyResolver dependencyResolver);
    }

    public interface IBrush<in TCanvas, in TArgs, in TPaint> : IBrush
        where TPaint : IPaint
    {
        void Update(TCanvas canvas, TArgs args, TPaint paint);
        bool CanStartApply(TCanvas canvas, TArgs args, TPaint paint);
        void StartApply(TCanvas canvas, TArgs args, TPaint paint);
        void UpdateApply(TCanvas canvas, TArgs args, TPaint paint);
        bool CanEndApply(TCanvas canvas, TArgs args, TPaint paint);
        void EndApply(TCanvas canvas, TArgs args, TPaint paint, IUndoRedoStack undoRedoStack);
        void OnInvalidStart(TCanvas canvas, TArgs args, TPaint paint);
        void OnCancellation(TCanvas canvas, TArgs args, TPaint paint);
        void OnInvalidEnd(TCanvas canvas, TArgs args, TPaint paint);
    }
}