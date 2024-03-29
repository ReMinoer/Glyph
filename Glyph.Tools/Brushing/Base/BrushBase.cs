﻿using Glyph.Composition;
using Glyph.Tools.UndoRedo;
using Niddle;

namespace Glyph.Tools.Brushing.Base
{
    public abstract class BrushBase<TCanvas, TBrushArgs, TPaint> : IBrush<TCanvas, TBrushArgs, TPaint>
        where TPaint : IPaint
    {
        public virtual void Update(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual bool CanStartApply(TCanvas canvas, TBrushArgs args, TPaint paint) => true;
        public virtual void StartApply(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void UpdateApply(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual bool CanEndApply(TCanvas canvas, TBrushArgs args, TPaint paint) => true;
        public virtual void EndApply(TCanvas canvas, TBrushArgs args, TPaint paint, IUndoRedoStack undoRedoStack) { }
        public virtual void OnInvalidStart(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void OnCancellation(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual void OnInvalidEnd(TCanvas canvas, TBrushArgs args, TPaint paint) { }
        public virtual IGlyphComponent CreateCursor(IDependencyResolver dependencyResolver) => null;
    }
}