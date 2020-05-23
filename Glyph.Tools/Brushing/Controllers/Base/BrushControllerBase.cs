using System;
using Glyph.Core;

namespace Glyph.Tools.Brushing.Controllers.Base
{
    public abstract class BrushControllerBase<TCanvas, TBrushArgs, TPaint> : GlyphObject, IBrushController<TCanvas, TBrushArgs, TPaint>, IIntegratedEditor<TCanvas>
        where TPaint : IPaint
    {
        public TCanvas Canvas { get; set; }
        public IBrush<TCanvas, TBrushArgs, TPaint> Brush { get; set; }
        public TPaint Paint { get; set; }
        public bool ApplyingBrush { get; private set; }

        object IIntegratedEditor.EditedObject => Canvas;
        TCanvas IIntegratedEditor<TCanvas>.EditedObject => Canvas;

        public event EventHandler ApplyStarted;
        public event EventHandler ApplyCancelled;
        public event EventHandler ApplyEnded;

        protected BrushControllerBase(GlyphResolveContext context)
            : base(context)
        {
            Schedulers.Update.Plan(UpdateLocal);
        }

        private void UpdateLocal(ElapsedTime elapsedtime)
        {
            if (!ApplyingBrush)
            {
                if (RequestingApplyStart && Canvas != null && Brush != null && Paint != null)
                {
                    TBrushArgs args = GetBrushArgs(Canvas);

                    if (Brush.CanStartApply(Canvas, args, Paint))
                    {
                        ApplyingBrush = true;
                        Brush.StartApply(Canvas, args, Paint);
                        ApplyStarted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Brush.OnInvalidStart(Canvas, args, Paint);
                    }
                }
            }
            else
            {
                if (RequestingApplyEnd)
                {
                    TBrushArgs args = GetBrushArgs(Canvas);
                    ApplyingBrush = false;

                    if (Brush.CanEndApply(Canvas, args, Paint))
                    {
                        Brush.EndApply(Canvas, args, Paint);
                        ApplyEnded?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Brush.OnInvalidEnd(Canvas, args, Paint);
                    }
                }
            }

            if (ApplyingBrush)
            {
                TBrushArgs args = GetBrushArgs(Canvas);

                if (RequestingCancellation)
                {
                    ApplyingBrush = false;

                    Brush.OnCancellation(Canvas, args, Paint);
                    ApplyCancelled?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Brush.UpdateApply(Canvas, args, Paint);
                }
            }
        }

        protected abstract bool RequestingApplyStart { get; }
        protected abstract bool RequestingApplyEnd { get; }
        protected abstract bool RequestingCancellation { get; }
        protected abstract TBrushArgs GetBrushArgs(TCanvas canvas);
    }
}