﻿namespace Glyph.Tools.Brushing
{
    public interface IPaint
    {
    }

    public interface IPaint<in TCanvas, in TArgs> : IPaint
    {
        bool CanApply(TCanvas canvas, TArgs args);
        void Apply(TCanvas canvas, TArgs args);
        object GetUndoData(TCanvas canvas, TArgs args);
        void Undo(TCanvas canvas, TArgs args, object undoData);
    }
}