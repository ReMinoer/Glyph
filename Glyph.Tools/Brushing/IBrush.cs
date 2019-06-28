namespace Glyph.Tools.Brushing
{
    public interface IBrush
    {
    }

    public interface IBrush<in TCanvas, in TArgs, in TPaint> : IBrush
        where TPaint : IPaint
    {
        bool CanStartApply(TCanvas canvas, TArgs args, TPaint paint);
        void StartApply(TCanvas canvas, TArgs args, TPaint paint);
        void UpdateApply(TCanvas canvas, TArgs args, TPaint paint);
        bool CanEndApply(TCanvas canvas, TArgs args, TPaint paint);
        void EndApply(TCanvas canvas, TArgs args, TPaint paint);
        void OnInvalidStart(TCanvas canvas, TArgs args, TPaint paint);
        void OnCancellation(TCanvas canvas, TArgs args, TPaint paint);
        void OnInvalidEnd(TCanvas canvas, TArgs args, TPaint paint);
    }
}